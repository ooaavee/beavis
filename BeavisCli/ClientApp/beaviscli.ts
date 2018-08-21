///<reference path="typings\globals/angular/index.d.ts" />
///<reference path="typings\globals/jquery/index.d.ts" />

namespace BeavisCli {
    const app: ng.IModule = angular.module("BeavisCli", []);

    interface IRequest {
        input: string;
    }

    interface IResponse {
        messages: IMessage[];
        statements: string[];
    }

    interface IMessage {
        text: string;
        type: string;
    }

    interface IUploader {
        input: any;
        file: IFileContent;
    }

    interface IFileContent {
        name: string;
        type: string;
        dataUrl: string;
    }

    interface IQueuedJob {
        key: string;
        statement: string;
    }

    class CliController {
        private uploader: IUploader;
        private jobQueue: IQueuedJob[] = [];

        static $inject = ["$rootScope", "$http"];

        constructor(private $rootScope: ng.IRootScopeService, private $http: ng.IHttpService) {
            window["$ctrl"] = this;

            // initialize the file uploader
            const input = document.querySelector("#uploader");
            input.addEventListener("change", () => {
                this.beginUpload();
            });
            this.uploader = { input: input, file: null };

            this.$rootScope.$on("terminal.mounted", (e, terminal) => {
                this.onMount(terminal);
            });

            this.$rootScope.$on("terminal.input", (e, input, terminal) => {
                this.processInput(input);
            });
        }

        /**
         * Occurs when the JQuery Terminal component has been mounted
         **/
        private onMount(terminal: any) {
            window["terminal"] = terminal;

            window["terminal"].completion = (terminal, command, callback) => {
                if (window["__terminal_completion"]) {
                    callback(window["__terminal_completion"]);
                }
            };

            this.freeze();

            this.$http.post<IResponse>("/beaviscli-api/initialize", null, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    this.onResponse(data);
                }).error((data, status) => {
                    this.onError(data);
                }).finally(() => {
                    this.awake();
                });
        }

        /**
         * Process JQuery terminal input events
         **/
        private processInput(input: string) {
            const job: IQueuedJob = this.popJob();
            if (job) {
                this.beginQueuedJob(job, input);
                return;
            }

            if (input.trim().length === 0) {
                return;
            }

            // triggers file uploading process
            if (input === "upload" && window["__upload_enabled"]) {
                this.uploader.input.click();
                return;
            }

            this.freeze();

            // send server request
            this.$http.post<IResponse>("/beaviscli-api/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    this.onResponse(data);
                }).error((data, status) => {
                    this.onError(data);
                }).finally(() => {
                    this.awake();
                });
        }

        /**
         * Begins file uploading
         **/
        private beginUpload() {
            const file = this.uploader.input.files.item(0);

            this.uploader.file = { name: file.name, type: file.type, dataUrl: null };

            var reader = new FileReader();
            reader.readAsDataURL(file);

            reader.onload = () => {
                this.uploader.file.dataUrl = reader.result;

                this.freeze();

                this.$http.post<IResponse>("/beaviscli-api/upload", JSON.stringify(this.uploader.file), { headers: { 'Content-Type': "application/json" } })
                    .success((data: IResponse) => {
                        this.onResponse(data);
                        $("#uploader").val("");
                    }).error((data, status) => {
                        this.onError(data);
                    }).finally(() => {
                        this.awake();
                    });

                this.uploader.file = null;
            };

            reader.onerror = error => {
                this.uploader.file = null;
                this.onError(error);
            };
        }

        /**
         * Queues a new job
         **/
        private queueJob(key: string, statement: string) {
            this.jobQueue.push({ key: key, statement: statement });
        }

        /**
         * Pops a job from the queue
         */
        private popJob(): IQueuedJob {
            let item: IQueuedJob = null;
            if (this.jobQueue.length > 0) {
                item = this.jobQueue[0];
                this.jobQueue.splice(0, 1);
            }
            return item;
        }

        /**
         * Begins a queued job
         */
        private beginQueuedJob(job: IQueuedJob, content: string) {
            this.beginJob(job.key, content);

            if (job.statement) {
                eval(job.statement);
            }
        }

        /**
         * Begins a new job
         **/
        private beginJob(key: string, content: any) {
            this.freeze();

            this.$http.post<IResponse>(`/beaviscli-api/job?key=${encodeURIComponent(key)}`, content, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    this.onResponse(data);
                }).error((data, status) => {
                    this.onError(data);
                }).finally(() => {
                    this.awake();
                });
        }

        /**
         * Handles responses from the server
         **/
        private onResponse(response: IResponse) {
            this.$rootScope.$emit("terminal.output", response.messages);

            for (let statement of response.statements) {
                eval(statement);
            }
        }

        /**
         * Handles errors
         */
        private onError(error) {
            alert(error);
            console.log(error);
            window["terminal"].error(error);
        }

        /**
         * Freeze terminal
         */
        private freeze() {
            window["terminal"].freeze(true);
        }

        /**
         * Awake terminal
         */
        private awake() {
            window["terminal"].freeze(false);
        }
    }
    app.controller("cli", CliController);

    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
        return {
            restrict: "A",
            link(scope, element, attrs) {

                // receive terminal input events and notify the CliController about that
                const terminal = element.terminal((input, terminal) => {
                    $rootScope.$emit("terminal.input", input, terminal);
                },
                    {
                        greetings: attrs.greetings || "",
                        completion(command, callback) {
                            if (window["__terminal_completion"]) {
                                callback(window["__terminal_completion"]);
                            }
                        }
                    });

                // notify CliController that we are ready to go!
                $rootScope.$emit("terminal.mounted", terminal);

                // receive terminal output evens from the CliController
                $rootScope.$on("terminal.output", (e, messages: IMessage[]) => {
                    for (let message of messages) {
                        let text: string = message.text;

                        if (text === "") {
                            text = "\n";
                        }

                        switch (message.type) {
                            case "Plain":
                                terminal.echo(text);
                                break;

                            case "Success":
                                terminal.echo(text, {
                                    finalize(div) {
                                        div.css("color", "#00ff00");
                                    }
                                });
                                break;

                            case "Error":
                                terminal.error(text);
                                break;
                        }
                    }
                });
            }
        };
    }]);
}