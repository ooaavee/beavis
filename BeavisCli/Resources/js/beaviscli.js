var BeavisCli;
(function (BeavisCli) {
    var app = angular.module("BeavisCli", []);
    var CliController = (function () {
        function CliController($rootScope, $http) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$http = $http;
            this.jobQueue = [];
            window["$ctrl"] = this;
            var input = document.querySelector("#uploader");
            input.addEventListener("change", function () {
                _this.beginUpload();
            });
            this.uploader = { input: input, file: null };
            this.$rootScope.$on("terminal.mounted", function (e, terminal) {
                _this.onMount(terminal);
            });
            this.$rootScope.$on("terminal.input", function (e, input, terminal) {
                _this.processInput(input);
            });
        }
        CliController.prototype.onMount = function (terminal) {
            var _this = this;
            window["terminal"] = terminal;
            window["terminal"].completion = function (terminal, command, callback) {
                if (window["__terminal_completion"]) {
                    callback(window["__terminal_completion"]);
                }
            };
            this.freeze();
            this.$http.post("/beaviscli-api/initialize", null, { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                _this.onResponse(data);
            }).error(function (data, status) {
                _this.onError(data);
            }).finally(function () {
                _this.awake();
            });
        };
        CliController.prototype.processInput = function (input) {
            var _this = this;
            var job = this.popJob();
            if (job) {
                this.beginQueuedJob(job, input);
                return;
            }
            if (input.trim().length === 0) {
                return;
            }
            if (input === "upload" && window["__upload_enabled"]) {
                this.uploader.input.click();
                return;
            }
            this.freeze();
            this.$http.post("/beaviscli-api/request", JSON.stringify({ input: input }), { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                _this.onResponse(data);
            }).error(function (data, status) {
                _this.onError(data);
            }).finally(function () {
                _this.awake();
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
                _this.freeze();
                _this.$http.post("/beaviscli-api/upload", JSON.stringify(_this.uploader.file), { headers: { 'Content-Type': "application/json" } })
                    .success(function (data) {
                    _this.onResponse(data);
                    $("#uploader").val("");
                }).error(function (data, status) {
                    _this.onError(data);
                }).finally(function () {
                    _this.awake();
                });
                _this.uploader.file = null;
            };
            reader.onerror = function (error) {
                _this.uploader.file = null;
                _this.onError(error);
            };
        };
        CliController.prototype.queueJob = function (key, statement) {
            this.jobQueue.push({ key: key, statement: statement });
        };
        CliController.prototype.popJob = function () {
            var item = null;
            if (this.jobQueue.length > 0) {
                item = this.jobQueue[0];
                this.jobQueue.splice(0, 1);
            }
            return item;
        };
        CliController.prototype.beginQueuedJob = function (job, content) {
            this.beginJob(job.key, content);
            if (job.statement) {
                eval(job.statement);
            }
        };
        CliController.prototype.beginJob = function (key, content) {
            var _this = this;
            this.freeze();
            this.$http.post("/beaviscli-api/job?key=" + encodeURIComponent(key), content, { headers: { 'Content-Type': "application/json" } })
                .success(function (data) {
                _this.onResponse(data);
            }).error(function (data, status) {
                _this.onError(data);
            }).finally(function () {
                _this.awake();
            });
        };
        CliController.prototype.onResponse = function (response) {
            this.$rootScope.$emit("terminal.output", response.messages);
            for (var _i = 0, _a = response.statements; _i < _a.length; _i++) {
                var statement = _a[_i];
                eval(statement);
            }
        };
        CliController.prototype.onError = function (error) {
            alert(error);
            console.log(error);
            window["terminal"].error(error);
        };
        CliController.prototype.freeze = function () {
            window["terminal"].freeze(true);
        };
        CliController.prototype.awake = function () {
            window["terminal"].freeze(false);
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
                        for (var _i = 0, messages_1 = messages; _i < messages_1.length; _i++) {
                            var message = messages_1[_i];
                            var text = message.text;
                            if (text === "") {
                                text = "\n";
                            }
                            switch (message.type) {
                                case "Plain":
                                    terminal.echo(text);
                                    break;
                                case "Success":
                                    terminal.echo(text, {
                                        finalize: function (div) {
                                            div.css("color", "#00ff00");
                                        }
                                    });
                                    break;
                                case "Error":
                                    terminal.error(text);
                                    break;
                            }
                        }
                    });
                }
            };
        }]);
})(BeavisCli || (BeavisCli = {}));
