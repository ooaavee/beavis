var BeavisCli;
(function (BeavisCli) {
    var app = angular.module("BeavisCli", []);
    var CliController = (function () {
        function CliController($rootScope, $http) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$http = $http;
            this.initUploader();
            this.$rootScope.$on("terminal.mounted", function (e, terminal) {
                _this.onMount(terminal);
            });
            this.$rootScope.$on("terminal.input", function (e, input, terminal) {
                _this.processInput(input);
            });
        }
        CliController.prototype.initUploader = function () {
            var _this = this;
            var input = document.querySelector("#uploader");
            input.addEventListener("change", function () {
                _this.beginUpload();
            });
            this.uploader = { input: input, file: null };
        };
        CliController.prototype.onMount = function (terminal) {
            var _this = this;
            this.terminal = terminal;
            this.terminal.completion = function (terminal, command, callback) {
                if (window["__terminal_completion"]) {
                    callback(window["__terminal_completion"]);
                }
            };
            this.$http.post("/beaviscli-fixed-api-segment/initialize", null, { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                _this.handleResponse(data, _this.terminal, _this);
            }).error(function (data, status) {
                _this.handleError(data, _this.terminal);
            });
        };
        CliController.prototype.processInput = function (input) {
            var _this = this;
            if (input === "upload" && window["__upload_enabled"]) {
                this.uploader.input.click();
                return;
            }
            this.$http.post("/beaviscli-fixed-api-segment/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                _this.handleResponse(data, _this.terminal, _this);
            }).error(function (data, status) {
                _this.handleError(data, _this.terminal);
            });
        };
        CliController.prototype.beginUpload = function () {
            var _this = this;
            var file = this.uploader.input.files.item(0);
            this.uploader.file = { name: file.name, type: file.type, dataUrl: null };
            var reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = function () {
                _this.uploader.file.dataUrl = reader.result;
                _this.$http.post("/beaviscli-fixed-api-segment/upload", JSON.stringify(_this.uploader.file), { headers: { 'Content-Type': "application/json" } })
                    .success(function (data) {
                    _this.handleResponse(data, _this.terminal, _this);
                    $('#uploader').val('');
                }).error(function (data, status) {
                    _this.handleError(data, _this.terminal);
                });
                _this.uploader.file = null;
            };
            reader.onerror = function (error) {
                _this.uploader.file = null;
                _this.handleError(error, _this.terminal);
            };
        };
        CliController.prototype.job = function (key, terminal) {
            var _this = this;
            this.$http.post("/beaviscli-fixed-api-segment/job?key=" + encodeURIComponent(key), null, { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                _this.handleResponse(data, terminal, _this);
            }).error(function (data, status) {
                _this.handleError(data, terminal);
            });
        };
        CliController.prototype.handleResponse = function (response, terminal, $ctrl) {
            this.$rootScope.$emit("terminal.output", response.messages);
            for (var i = 0; i < response.statements.length; i++) {
                eval(response.statements[i]);
            }
        };
        CliController.prototype.handleError = function (error, terminal) {
            console.log(error);
            terminal.error(error);
        };
        CliController.$inject = ["$rootScope", "$http"];
        return CliController;
    }());
    app.controller("cli", CliController);
    app.directive("angularTerminal", ["$rootScope", function ($rootScope) {
            return {
                restrict: "A",
                link: function (scope, element, attrs) {
                    var terminal = element.terminal(function (input, terminal) {
                        var value = input.trim();
                        if (value.length > 0) {
                            $rootScope.$emit("terminal.input", input, terminal);
                        }
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
})(BeavisCli || (BeavisCli = {}));
