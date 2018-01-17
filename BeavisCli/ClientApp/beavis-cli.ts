///<reference path="typings\globals/angular/index.d.ts" />
///<reference path="typings\globals/jquery/index.d.ts" />

namespace BeavisCli {

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

    ////class Terminal {
    ////    constructor(public handle: any) {
    ////    }

    ////    /**
    ////     * Clears terminal history.
    ////     */
    ////    public clearHistory() {
    ////        this.handle.history().clear();
    ////    }
    ////}


    const app: ng.IModule = angular.module("BeavisCli", []);


    class CliService {
        static $inject = ["$rootScope", "$http"];

        constructor(private $rootScope: ng.IRootScopeService, private $http: ng.IHttpService) {
            //let self = this;

            this.$rootScope.$on("terminal.mounted", (e, terminal) => {
                this.onMount(terminal);
            });

            this.$rootScope.$on("terminal.input", (e, input, terminal) => {
                this.handleInput(input, terminal);
            });

        }

        private onMount(terminal: any) {
            let self = this;
            self.$http.post<IResponse>("/beavis-cli/api/init", null, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    self.handleMessages(data.messages);
                    self.handleStatements(data.statements, terminal);
                }).error((data, status) => {
                    debugger;
                });
        }

        /**
         * Handles terminal input events.
         */
        private handleInput(input: string, terminal: any) {
            let self = this;

            if (input.trim().length === 0) {
                return;
            }

            self.$http.post<IResponse>("/beavis-cli/api/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    self.handleMessages(data.messages);
                    self.handleStatements(data.statements, terminal);
                }).error((data, status) => {
                    debugger;
                });
        }

        private handleMessages(messages: IMessage[]) {
            this.$rootScope.$emit("terminal.output", messages);
        }

        private handleStatements(statements: string[], terminal: any) {
            for (let i = 0; i < statements.length; i++) {
                eval(statements[i]);
                //                self.evalTerminalStatement(data.statements[i], evt.terminal.handle);
            }
        }


        //private evalTerminalStatement(statement: string, terminal: any) {
        //    eval(statement);
        //}
    }
    app.service("backend", CliService);


    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
        return {
            restrict: "A",
            link(scope, element, attrs) {

                //const namespace = `terminal.${attrs.angularTerminal || "default"}`;

                /**
                 * Receive terminal input events and notify the CliService about that
                 */
                const terminal = element.terminal((input, terminal) => {
                    $rootScope.$emit("terminal.input", input, terminal);
                }, { greetings: attrs.greetings || "" });

                /**
                 * Notify CliService that we are ready to go!
                 */
                $rootScope.$emit("terminal.mounted", terminal);

                /**
                 * Receive terminal output evens from the CliService
                 */
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