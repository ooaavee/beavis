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

    class CliController {
        private uploader: IUploader;
        private terminal: any;

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

        /***************************************************************************************
         * Initializes the file uploader.
         ***************************************************************************************/
        private initUploader() {
            var input = document.querySelector("#uploader");

            input.addEventListener("change", () => {
                this.beginUpload();
            });

            this.uploader = { input: input, file: null };
        }

        /***************************************************************************************
         * Occurs when the JQuery Terminal component has been mounted.
         ***************************************************************************************/
        private onMount(terminal: any) {
            this.terminal = terminal;

            this.terminal.completion = (terminal, command, callback) => {
                if (window["__terminal_completion"]) {
                    callback(window["__terminal_completion"]);
                }
            };

            this.$http.post<IResponse>("/beavis-cli/api/initialize", null, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    this.handleResponse(data, this.terminal, this);
                }).error((data, status) => {
                    debugger;
                });
        }

        /***************************************************************************************
         * Process JQuery terminal input events.
         ***************************************************************************************/
        private processInput(input: string) {
            // triggers file uploading process
            if (input === "upload" && window["__upload_enabled"]) {
                this.uploader.input.click();
                return;
            }

            // send server request
            this.$http.post<IResponse>("/beavis-cli/api/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    this.handleResponse(data, this.terminal, this);
                }).error((data, status) => {
                    debugger;
                });
        }

        /***************************************************************************************
         * Begins file uploading.
         ***************************************************************************************/
        private beginUpload() {
            var file = this.uploader.input.files.item(0);

            this.uploader.file = { name: file.name, type: file.type, dataUrl: null };

            var reader = new FileReader();
            reader.readAsDataURL(file);

            reader.onload = () => {
                this.uploader.file.dataUrl = reader.result;

                this.$http.post<IResponse>("/beavis-cli/api/upload", JSON.stringify(this.uploader.file), { headers: { 'Content-Type': "application/json" } })
                    .success((data: IResponse) => {
                        this.handleResponse(data, this.terminal, this);
                    }).error((data, status) => {
                        debugger;
                    });

                this.uploader.file = null;
            };

            reader.onerror = error => {
                this.uploader.file = null;
                console.log('Error: ', error);
                debugger;
            };
        }

        /***************************************************************************************
         * Begins a new job.
         ***************************************************************************************/
        private beginJob(key: string, terminal: any) {
            this.$http.post<IResponse>("/beavis-cli/api/job?key=" + encodeURIComponent(key), null, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    this.handleResponse(data, terminal, this);
                }).error((data, status) => {
                    debugger;
                });
        }

        /***************************************************************************************
         * Handles a response from the server.
         ***************************************************************************************/
        private handleResponse(response: IResponse, terminal: any, $ctrl: CliController) {
            // 1. Write terminal output messages.
            this.$rootScope.$emit("terminal.output", response.messages);

            // 2. Eval JavaScript statements.
            for (let i = 0; i < response.statements.length; i++) {
                eval(response.statements[i]);
            }
        }
    }
    app.controller("cli", CliController);

    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
        return {
            restrict: "A",
            link(scope, element, attrs) {

                // Receive terminal input events and notify the CliController about that
                const terminal = element.terminal((input, terminal) => {
                    var value: string = input.trim();
                    if (value.length > 0) {
                        $rootScope.$emit("terminal.input", input, terminal);

                    }
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

}