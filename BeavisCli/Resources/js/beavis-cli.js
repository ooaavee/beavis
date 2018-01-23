var BeavisCli;
(function (BeavisCli) {
    var app = angular.module("BeavisCli", []);
    var CliService = (function () {
        function CliService($rootScope, $http) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$http = $http;
            this.initUploader();
            this.$rootScope.$on("terminal.mounted", function (e, terminal) {
                _this.onMount(terminal);
            });
            this.$rootScope.$on("terminal.input", function (e, input, terminal) {
                var value = input.trim();
                if (value.length > 0) {
                    _this.handleInput(value);
                }
            });
        }
        CliService.prototype.initUploader = function () {
            var self = this;
            self.uploader = {
                input: document.querySelector("#uploader"),
                file: null
            };
            self.uploader.input.addEventListener("change", function () {
                self.beginUpload();
            });
        };
        CliService.prototype.onMount = function (terminal) {
            var self = this;
            self.terminal = terminal;
            self.terminal.completion = function (terminal, command, callback) {
                if (window["__terminal_completion"]) {
                    callback(window["__terminal_completion"]);
                }
            };
            self.$http.post("/beavis-cli/api/initialize", null, { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                self.handleResponse(data, self.terminal, self);
            }).error(function (data, status) {
                debugger;
            });
        };
        CliService.prototype.handleInput = function (input) {
            var self = this;
            if (input === "upload") {
                self.triggerUpload();
                return;
            }
            self.$http.post("/beavis-cli/api/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                self.handleResponse(data, self.terminal, self);
            }).error(function (data, status) {
                debugger;
            });
        };
        CliService.prototype.triggerUpload = function () {
            var self = this;
            self.uploader.input.click();
        };
        CliService.prototype.beginUpload = function () {
            var self = this;
            var file = self.uploader.input.files.item(0);
            self.uploader.file = { name: file.name, type: file.type, dataUrl: null };
            var reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = function () {
                self.uploader.file.dataUrl = reader.result;
                debugger;
            };
            reader.onerror = function (error) {
                self.uploader.file = null;
                console.log('Error: ', error);
                debugger;
            };
        };
        CliService.prototype.beginJob = function (key, terminal) {
            var self = this;
            self.$http.post("/beavis-cli/api/job?key=" + encodeURIComponent(key), null, { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                self.handleResponse(data, terminal, self);
            }).error(function (data, status) {
                debugger;
            });
        };
        CliService.prototype.handleResponse = function (response, terminal, service) {
            this.$rootScope.$emit("terminal.output", response.messages);
            for (var i = 0; i < response.statements.length; i++) {
                eval(response.statements[i]);
            }
        };
        return CliService;
    }());
    CliService.$inject = ["$rootScope", "$http"];
    app.service("backend", CliService);
    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
            return {
                restrict: "A",
                link: function (scope, element, attrs) {
                    var terminal = element.terminal(function (input, terminal) {
                        $rootScope.$emit("terminal.input", input, terminal);
                    }, {
                        greetings: attrs.greetings || "",
                        completion: function (command, callback) {
                            if (window["__terminal_completion"]) {
                                callback(window["__terminal_completion"]);
                            }
                        }
                    });
                    $rootScope.$emit("terminal.mounted", terminal);
                    $rootScope.$on("terminal.output", function (e, messages) {
                        for (var i = 0; i < messages.length; i++) {
                            var text = messages[i].text;
                            if (text === "") {
                                text = "\n";
                            }
                            switch (messages[i].type) {
                                case "information":
                                    terminal.echo(text);
                                    break;
                                case "success":
                                    terminal.echo(text, {
                                        finalize: function (div) {
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
    var CliController = (function () {
        function CliController(backend) {
            this.backend = backend;
        }
        return CliController;
    }());
    CliController.$inject = ["backend"];
    app.controller("cli", CliController);
})(BeavisCli || (BeavisCli = {}));
