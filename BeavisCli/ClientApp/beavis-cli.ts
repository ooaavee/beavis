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
        file: IUploaderFile;
    }

    interface IUploaderFile {
        name: string;
        type: string;
        dataUrl: string;
    }


    class CliService {
        static $inject = ["$rootScope", "$http"];

        private uploader: IUploader;
        private terminal: any;


        constructor(private $rootScope: ng.IRootScopeService, private $http: ng.IHttpService) {
            this.initUploader();

            this.$rootScope.$on("terminal.mounted", (e, terminal) => {
                this.onMount(terminal);
            });

            this.$rootScope.$on("terminal.input", (e, input, terminal) => {
                var value = input.trim();
                if (value.length > 0) {
                    this.handleInput(value);
                }
            });
        }


        /**
         * Initializes the file uploader.
         */
        private initUploader() {
            var self = this;

            self.uploader = {
                input: document.querySelector("#uploader"),
                file: null
            };

            self.uploader.input.addEventListener("change", () => {
                self.beginUpload();
            });
        }


        /**
         * Occurs when the JQuery Terminal component has been mounted.
         */
        private onMount(terminal: any) {
            let self = this;

            self.terminal = terminal;

            self.terminal.completion = (terminal, command, callback) => {
                if (window["__terminal_completion"]) {
                    callback(window["__terminal_completion"]);
                }
            };

            self.$http.post<IResponse>("/beavis-cli/api/initialize", null, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    self.handleResponse(data, self.terminal, self);
                }).error((data, status) => {
                    debugger;
                });
        }


        /**
         * Handles JQuery terminal input events.
         */
        private handleInput(input: string) {
            let self = this;

            if (input === "upload") {
                self.triggerUpload();
                return;
            }

            self.$http.post<IResponse>("/beavis-cli/api/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    self.handleResponse(data, self.terminal, self);
                }).error((data, status) => {
                    debugger;
                });
        }


        /**
         * Triggers file uploading process.
         */
        private triggerUpload() {
            var self = this;
            self.uploader.input.click();
        }


        /**
         * Begins file uploading.
         */
        private beginUpload() {
            var self = this;

            var file = self.uploader.input.files.item(0);

            self.uploader.file = { name: file.name, type: file.type, dataUrl: null };

            var reader = new FileReader();
            reader.readAsDataURL(file);

            reader.onload = () => {
                self.uploader.file.dataUrl = reader.result;

                // TODO: Kutsu omaa palvelua...

                debugger;
            };

            reader.onerror = error => {
                self.uploader.file = null;
                console.log('Error: ', error);
                debugger;
            };
        }


        /**
         * Begins a new job.
         */
        private beginJob(key: string, terminal: any) {
            let self = this;

            self.$http.post<IResponse>("/beavis-cli/api/job?key=" + encodeURIComponent(key), null, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    self.handleResponse(data, terminal, self);
                }).error((data, status) => {
                    debugger;
                });
        }


        /**
         * Handles a response from the server.
         */
        private handleResponse(response: IResponse, terminal: any, service: CliService) {
            // 1. Write terminal output messages.
            this.$rootScope.$emit("terminal.output", response.messages);

            // 2. Eval JavaScript statements.
            for (let i = 0; i < response.statements.length; i++) {
                eval(response.statements[i]);
            }
        }

    }
    app.service("backend", CliService);


    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
        return {
            restrict: "A",
            link(scope, element, attrs) {

                // Receive terminal input events and notify the CliService about that
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

                // Notify CliService that we are ready to go!
                $rootScope.$emit("terminal.mounted", terminal);

                // Receive terminal output evens from the CliService
                $rootScope.$on("terminal.output", (e, messages: IMessage[]) => {
                    for (let i = 0; i < messages.length; i++) {

                        let text: string = messages[i].text;
                        if (text === "") {
                            text = "\n";
                        }

                        switch (messages[i].type) {
                            case "information":
                                terminal.echo(text);
                                break;

                            case "success":
                                terminal.echo(text, {
                                    finalize(div) {
                                        div.css("color", "#00ff00");
                                    }
                                });
                                break;

                            case "error":
                                terminal.error(text);
                                break;
                        }

                    }
                });

            }
        };
    }]);


    class CliController {
        static $inject = ["backend"];
        constructor(private backend: CliService) { }
    }
    app.controller("cli", CliController);


}