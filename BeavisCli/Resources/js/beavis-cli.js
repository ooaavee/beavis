var BeavisCli;
(function (BeavisCli) {
    var Terminal = (function () {
        function Terminal(handle) {
            this.handle = handle;
        }
        Terminal.prototype.clearHistory = function () {
            this.handle.history().clear();
        };
        return Terminal;
    }());
    var app = angular.module('BeavisCli', []);
    var TerminalService = (function () {
        function TerminalService($rootScope, $http) {
            this.$rootScope = $rootScope;
            this.$http = $http;
            var self = this;
            self.$rootScope.$on('terminal.main', function (e, input, terminal) {
                self.handleInput(input, new Terminal(terminal));
            });
        }
        TerminalService.prototype.welcome = function () {
            var self = this;
            self.$http.post("/beavis/api/welcome", null, { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                self.handleMessages(data.messages);
                self.handleStatements(data.statements);
            }).error(function (data, status) {
                debugger;
            });
        };
        TerminalService.prototype.handleInput = function (input, terminal) {
            var self = this;
            if (input.trim().length === 0) {
                return;
            }
            self.$http.post("/beavis/api/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                self.handleMessages(data.messages);
                self.handleStatements(data.statements);
            }).error(function (data, status) {
                debugger;
            });
        };
        TerminalService.prototype.handleMessages = function (messages) {
            this.$rootScope.$emit("terminal.main.messages", messages);
        };
        TerminalService.prototype.handleStatements = function (statements) {
            for (var i = 0; i < statements.length; i++) {
                eval(statements[i]);
            }
        };
        return TerminalService;
    }());
    TerminalService.$inject = ["$rootScope", "$http"];
    app.service("terminalService", TerminalService);
    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
            return {
                restrict: "A",
                link: function (scope, element, attrs) {
                    var namespace = "terminal." + (attrs.angularTerminal || "default");
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
            terminalService.welcome();
        }
        return TerminalController;
    }());
    TerminalController.$inject = ["terminalService"];
    app.controller("terminalController", TerminalController);
})(BeavisCli || (BeavisCli = {}));
