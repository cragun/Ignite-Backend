(function () {
    function HeaderParameter(name, value) {
        var self = this;

        self.tagName = name + "Tag";
        self.name = name;
        self._value = ko.observable(value);
        self.value = ko.computed({
            read: self._value,
            write: function (newValue) {
                options.saveAdditionalHeaderDescription(new options.HeaderDescription(self.name, newValue));
                self._value(newValue);
            }
        });
    }

    var options = DayCare.HelpPage.Options;

    function ViewModel() {
        var self = this;

        self.userName = ko.observable();
        self.password = ko.observable();
        self._authToken = ko.observable();
        self.authToken = ko.computed({
            read: self._authToken,
            write: function (value) {
                options.saveAuthToken(value);
                self._authToken(value);
            }
        });

        self.additionalHeaders = ko.observableArray();
        self.newAdditionalHeaderName = ko.observable();

        self.addNewAdditionalHeader = function () {
            var newAdditionalHeaderName = self.newAdditionalHeaderName();
            self.additionalHeaders.push(new HeaderParameter(newAdditionalHeaderName, null));
            options.saveAdditionalHeaderDescription(new options.HeaderDescription(newAdditionalHeaderName, null));
        }

        self.removeAdditioanlHeaderValue = function (additionalHeader) {
            options.removeAdditionalHeaderDescription(additionalHeader.name);
            self.additionalHeaders.remove(additionalHeader);
        };

        self.getAuthenticationToken = function () {
            /* TODO: write this code after the authentication logic was implemented
            callApiLogin(self.userName, self.password, function (token) {
                console.log(token);
            });*/
        }
    }

    var viewModel = new ViewModel();

    viewModel.authToken(options.getAuthToken());
    options.getAdditionalHeaderDescriptions().forEach(function (headerDescription) {
        viewModel.additionalHeaders.push(new HeaderParameter(headerDescription.name, headerDescription.value));
    });

    ko.applyBindings(viewModel);
})();