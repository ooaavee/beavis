var BeavisCli;
(function (BeavisCli) {
    var Terminal = (function () {
        function Terminal(handle) {
            this.handle = handle;
        }
        Terminal.prototype.clearHistory = function () {
            var self = this;
            var history = self.handle.history();
            history.clear();
        };
        return Terminal;
    }());
    var app = angular.module("BeavisCli", []);
    var TerminalService = (function () {
        function TerminalService($rootScope, $http) {
            this.$rootScope = $rootScope;
            this.$http = $http;
            var self = this;
            self.$rootScope.$on("terminal.main", function (e, input, terminal) {
                self.handleTerminalInput({ value: input, terminal: new Terminal(terminal) });
            });
        }
        TerminalService.prototype.handleTerminalInput = function (evt) {
            var self = this;
            if (evt.value.trim().length > 0) {
                self.$http.post("/jemma/command", JSON.stringify({ input: evt.value }), { headers: { 'Content-Type': "application/json" } })
                    .success(function (data) {
                    self.$rootScope.$emit("terminal.main.messages", data.messages);
                    for (var i = 0; i < data.statements.length; i++) {
                        self.evalTerminalStatement(data.statements[i], evt.terminal.handle);
                    }
                }).error(function (data, status) {
                    debugger;
                });
            }
        };
        TerminalService.prototype.evalTerminalStatement = function (statement, terminal) {
            eval(statement);
        };
        return TerminalService;
    }());
    TerminalService.$inject = ["$rootScope", "$http"];
    app.service("terminalService", TerminalService);
    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
            return {
                restrict: "A",
                link: function (scope, element, attrs) {
                    var namespace = "terminal.main";
                    var terminal = element.terminal(function (input, terminal) {
                        $rootScope.$emit(namespace, input, terminal);
                    }, { greetings: attrs.greetings || "" });
                    $rootScope.$on(namespace + ".messages", function (e, messages) {
                        var messageCount = messages.length;
                        for (var i = 0; i < messageCount; i++) {
                            var message = messages[i];
                            var text = message.text;
                            if (text === "") {
                                text = "\n";
                            }
                            switch (message.type) {
                                case "information":
                                    terminal.echo(text);
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
    var TerminalController = (function () {
        function TerminalController(terminalService) {
            this.terminalService = terminalService;
        }
        return TerminalController;
    }());
    TerminalController.$inject = ["terminalService"];
    app.controller("terminalController", TerminalController);
})(BeavisCli || (BeavisCli = {}));
