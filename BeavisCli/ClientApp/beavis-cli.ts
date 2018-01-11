///<reference path="typings\globals/angular/index.d.ts" />
///<reference path="typings\globals/jquery/index.d.ts" />

namespace BeavisCli {

    interface IInputEvent {
        value: string;
        terminal: Terminal;
    }

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
            let self = this;
            let history = self.handle.history();
            history.clear();
        }
    }


    const app: ng.IModule = angular.module('BeavisCli', []);


    class TerminalService {
        static $inject = ["$rootScope", "$http"];

        constructor(private $rootScope: ng.IRootScopeService, private $http: ng.IHttpService) {
            let self = this;
            self.$rootScope.$on('terminal.main', function (e, input, terminal) {
                self.handleTerminalInput({ value: input, terminal: new Terminal(terminal) });
            });
        }

        /**
         * Handles terminal input events.
         */
        private handleTerminalInput(evt: IInputEvent) {
            let self = this;
            if (evt.value.trim().length > 0) {
                self.$http.post<IResponse>("/beavis/request", JSON.stringify({ input: evt.value }), { headers: { 'Content-Type': "application/json" } })
                    .success((data: IResponse) => {

                        // 1. display terminal messages
                        self.$rootScope.$emit("terminal.main.messages", data.messages);

                        // 2. evaluate statements received from the server
                        for (let i = 0; i < data.statements.length; i++) {
                            self.evalTerminalStatement(data.statements[i], evt.terminal.handle);
                        }

                    }).error((data, status) => {
                        debugger;
                    });
            }
        }

        private evalTerminalStatement(statement: string, terminal: any) {
            eval(statement);
        }
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
                    }
                });
            }
        };
    }]);


    class TerminalController {
        static $inject = ["terminalService"];
        constructor(private terminalService: TerminalService) {
        }
    }
    app.controller("terminalController", TerminalController);

}