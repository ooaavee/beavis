///<reference path="typings\globals/angular/index.d.ts" />
///<reference path="typings\globals/jquery/index.d.ts" />

namespace BeavisCli {

    interface ITerminalInputEvent {
        value: string;
        terminal: Terminal;
    }

    interface ICliRequest {
        input: string;
    }
     
    interface ICliResponse {
        messages: IResponseMessage[];
        statements: string[];
    }

    interface IResponseMessage {
        text: string;
        type: string;
    }


    class Terminal {
        constructor(public handle: any) {
        }

        /**
         * Clears terminal history.
         */
        clearHistory() {
            let self = this;
            let history = self.handle.history();
            history.clear();
        }
    }


    const app: ng.IModule = angular.module("BeavisCli", []);

    class TerminalService {
        static $inject = ["$rootScope", "$http"];

        constructor(private $rootScope: ng.IRootScopeService, private $http: ng.IHttpService) {
            let self = this;
            self.$rootScope.$on("terminal.main", function (e, input, terminal) {
                self.handleTerminalInput({ value: input, terminal: new Terminal(terminal) });
            });
        }

        /**
         * Handles terminal input events.
         */
        private handleTerminalInput(evt: ITerminalInputEvent) {
            let self = this;
            if (evt.value.trim().length > 0) {
                self.$http.post<ICliResponse>("/jemma/command", JSON.stringify({ input: evt.value }), { headers: { 'Content-Type': "application/json" } })
                    .success((data: ICliResponse) => {

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
                let namespace = "terminal.main";

                let terminal = element.terminal(function (input, terminal) {
                    $rootScope.$emit(namespace, input, terminal);
                }, { greetings: attrs.greetings || "" });

                $rootScope.$on(namespace + ".messages", function (e, messages: IResponseMessage[]) {
                    let messageCount: number = messages.length;

                    for (let i = 0; i < messageCount; i++) {

                        let message: IResponseMessage = messages[i];
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
        constructor(readonly terminalService: TerminalService) {
        }
    }
    app.controller("terminalController", TerminalController);

}