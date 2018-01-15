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

    class Terminal {
        constructor(public handle: any) {
        }

        /**
         * Clears terminal history.
         */
        public clearHistory() {
            this.handle.history().clear();
        }
    }


    const app: ng.IModule = angular.module('BeavisCli', []);


    class TerminalService {
        static $inject = ["$rootScope", "$http"];

        constructor(private $rootScope: ng.IRootScopeService, private $http: ng.IHttpService) {
            let self = this;
            self.$rootScope.$on('terminal.main', function (e, input, terminal) {
                self.handleInput(input, new Terminal(terminal));
            });
        }

        welcome() {
            let self = this;
            self.$http.post<IResponse>("/beavis/api/welcome", null, { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    self.handleMessages(data.messages);
                    self.handleStatements(data.statements);
                }).error((data, status) => {
                    debugger;
                });
        }

        /**
         * Handles terminal input events.
         */
        private handleInput(input: string, terminal: Terminal) {
            let self = this;

            if (input.trim().length === 0) {
                return;
            }

            self.$http.post<IResponse>("/beavis/api/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success((data: IResponse) => {
                    self.handleMessages(data.messages);
                    self.handleStatements(data.statements);
                }).error((data, status) => {
                    debugger;
                });
        }

        private handleMessages(messages: IMessage[]) {
            this.$rootScope.$emit("terminal.main.messages", messages);
        }

        private handleStatements(statements: string[]) {
            for (let i = 0; i < statements.length; i++) {
                eval(statements[i]);
//                self.evalTerminalStatement(data.statements[i], evt.terminal.handle);
            }
        }


        //private evalTerminalStatement(statement: string, terminal: any) {
        //    eval(statement);
        //}
    }
    app.service("terminalService", TerminalService);


    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
        return {
            restrict: "A",
            link: function (scope, element, attrs) {
                let namespace = "terminal." + (attrs.angularTerminal || "default");

                let terminal = element.terminal(function (input, terminal) {
                    $rootScope.$emit(namespace, input, terminal);
                }, { greetings: attrs.greetings || "" });

                $rootScope.$on(namespace + ".messages", function (e, messages: IMessage[]) {
                    let messageCount: number = messages.length;

                    for (let i = 0; i < messageCount; i++) {

                        let message: IMessage = messages[i];
                        let text: string = message.text;

                        if (text === "") {
                            text = "\n";
                        }

                        switch (message.type) {
                            case "information":
                                terminal.echo(text);

                                //terminal.echo('[[;#00ff00;]' + text + ']' + "Tämä on normaalia tekstiä normaalillä värillä!!!");

                                break;
                            case "error":
                                terminal.error(text);
                                break;
                            default:
                                debugger;
                        }


                        //$('html,body').animate({ scrollTop: document.body.scrollHeight }, "fast");
                        //window.scrollBy(0, document.body.scrollHeight || document.documentElement.scrollHeight);

                    }
                });
            }
        };
    }]);


    class TerminalController {
        static $inject = ["terminalService"];

        constructor(private terminalService: TerminalService) {
            terminalService.welcome();
        }

    }
    app.controller("terminalController", TerminalController);

}