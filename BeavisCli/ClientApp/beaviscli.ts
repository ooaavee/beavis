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
        private terminal: any;
        private jobQueue: IQueuedJob[] = [];

        static $inject = ["$rootScope", "$http"];

        constructor(private $rootScope: ng.IRootScopeService, private $http: ng.IHttpService) {
            this.initUploader();

            this.$rootScope.$on("terminal.mounted", (e, terminal) => {
                this.onMount(terminal);
            });

            this.$rootScope.$on("terminal.input", (e, input, terminal) => {
                this.processInput(input);
            });
        }

        /**
         * Initializes the file uploader
         **/
        private initUploader() {
            const input = document.querySelector("#uploader");

            input.addEventListener("change", () => {
                this.beginUpload();
            });

            this.uploader = { input: input, file: null };
        }

        /**
         * Occurs when the JQuery Terminal component has been mounted
         **/
        private onMount(terminal: any) {
            this.terminal = terminal;

            this.terminal.completion = (terminal, command, callback) => {
                if (window["__terminal_completion"]) {
                    callback(window["__terminal_completion"]);
                }
            };

            this.$http.post<IResponse>("/beaviscli-api/initialize", null, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    this.handleResponse(data, this.terminal, this);
                }).error((data, status) => {
                    this.handleError(data, this.terminal);
                });
        }

        /**
         * Process JQuery terminal input events
         **/
        private processInput(input: string) {
            const job: IQueuedJob = this.popJob();
            if (job) {
                this.beginQueuedJob(job, input, this.terminal);
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

            // send server request
            this.$http.post<IResponse>("/beaviscli-api/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    this.handleResponse(data, this.terminal, this);
                }).error((data, status) => {
                    this.handleError(data, this.terminal);
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

                this.$http.post<IResponse>("/beaviscli-api/upload", JSON.stringify(this.uploader.file), { headers: { 'Content-Type': "application/json" } })
                    .success((data: IResponse) => {
                        this.handleResponse(data, this.terminal, this);
                        $("#uploader").val("");
                    }).error((data, status) => {
                        this.handleError(data, this.terminal);
                    });

                this.uploader.file = null;
            };

            reader.onerror = error => {
                this.uploader.file = null;
                this.handleError(error, this.terminal);
            };
        }

        /**
         * Queues a new job
         **/
        private queueJob(key: string, statement: string) {
            this.jobQueue.push({ key: key, statement: statement });
        }

        private popJob(): IQueuedJob {
            let item: IQueuedJob = null;
            if (this.jobQueue.length > 0) {
                item = this.jobQueue[0];
                this.jobQueue.splice(0, 1);
            }
            return item;
        }

        private beginQueuedJob(job: IQueuedJob, content: string, terminal: any) {
            this.beginJob(job.key, this.terminal, content);
            
            if (job.statement) {
                eval(job.statement);
            }
        }

        /**
         * Begins a new job
         **/
        private beginJob(key: string, terminal: any, content: any) {
            this.$http.post<IResponse>(`/beaviscli-api/job?key=${encodeURIComponent(key)}`, content, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    this.handleResponse(data, terminal, this);
                }).error((data, status) => {
                    this.handleError(data, terminal);
                });
        }

        /**
         * Handles a response from the server
         **/
        private handleResponse(response: IResponse, terminal: any, $ctrl: CliController) {
            // write terminal output messages
            this.$rootScope.$emit("terminal.output", response.messages);

            // eval JavaScript statements
            for (let js of response.statements) {
                eval(js);
            }
        }

        private handleError(error, terminal: any) {
            console.log(error);
            terminal.error(error);
        }
    }
    app.controller("cli", CliController);

    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
        return {
            restrict: "A",
            link(scope, element, attrs) {

                // Receive terminal input events and notify the CliController about that
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

                // Notify CliController that we are ready to go!
                $rootScope.$emit("terminal.mounted", terminal);

                // Receive terminal output evens from the CliController
                $rootScope.$on("terminal.output", (e, messages: IMessage[]) => {
                    for (let i = 0; i < messages.length; i++) {

                        let text: string = messages[i].text;
                        if (text === "") {
                            text = "\n";
                        }

                        switch (messages[i].type) {
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