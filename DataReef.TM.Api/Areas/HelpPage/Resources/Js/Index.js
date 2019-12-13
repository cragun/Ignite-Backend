if (typeof DayCare.HelpPage.Persistence === 'undefined') {
    console.warn('Persistence module is not present. Settings will not be saved.');
}

(function () {
    //#region ViewModel(s)
    var APIRESOURCETYPE = {
        CONTROLLER: "Controller",
        COMPLEXTYPE: "ComplexType",
        ENUMERABLE: "Enum"
    };

    function ApiDescription(name, description, url, apiType) {
        var self = this;
        this.name = name;
        this.description = description;
        this.url = url;
        this.apiType = apiType;

        this.visible = ko.observable(false);

        this.getTypeColorClass = ko.pureComputed(function () {
            if (self.apiType == APIRESOURCETYPE.CONTROLLER) return "success";
            if (self.apiType == APIRESOURCETYPE.COMPLEXTYPE) return "info";
            if (self.apiType == APIRESOURCETYPE.ENUMERABLE) return "warning";
            return "";
        });
        this.openApiDetails = function () {
            window.location.href = this.url;
        }
    };

    function ViewModel() {
        var self = this;
        this._searchPhrase = ko.observable();
        this._showControllers = ko.observable();
        this._showModels = ko.observable();

        this.apiDescriptions = ko.observableArray([]);

        this.showControllersComputed = ko.computed({
            read: this._showControllers,
            write: function (value) {
                self._showControllers(value);
                self.searchApiData();
                persistSettings("ShowControllers", value);
            }
        });
        this.showModelsComputed = ko.computed({
            read: this._showModels,
            write: function (value) {
                self._showModels(value);
                self.searchApiData();
                persistSettings("ShowModels", value);
            }
        });
        this.searchPhraseComputed = ko.computed({
            read: this._searchPhrase,
            write: function (value) {
                self._searchPhrase(value);
                self.searchApiData();
            }
        });
    };

    ViewModel.prototype.searchApiData = function () {
        var self = this;
        this.apiDescriptions().forEach(function (apiDescription) {
            if (!self._searchPhrase()) {
                if ((apiDescription.apiType == APIRESOURCETYPE.CONTROLLER && self._showControllers()) ||
                    ((apiDescription.apiType == APIRESOURCETYPE.COMPLEXTYPE || apiDescription.apiType == APIRESOURCETYPE.ENUMERABLE) && self._showModels()))
                    apiDescription.visible(true);
                else apiDescription.visible(false);
            }
            else if (apiDescription.name.toUpperCase().indexOf(self._searchPhrase().toUpperCase()) > -1) {
                apiDescription.visible(true);
            } else {
                apiDescription.visible(false);
            }
        });
    }
    //#endregion

    //#region Persistence
    var persistedHeaderValuesPrefix = "HelpIndex-";
    var persistence = DayCare.HelpPage.Persistence;
    var persistSettings = function (name, value) {
        if (typeof persistence === 'undefined')
            return;

        persistence.persist(persistedHeaderValuesPrefix + name, value);
    }

    var retreaveSetting = function (name) {
        if (typeof persistence === 'undefined')
            return null;

        return persistence.retrieve(persistedHeaderValuesPrefix + name);
    }
    //#endregion

    //#region Private helper methods
    var apiTypeOrderWeight = function (apiType) {
        if (apiType == APIRESOURCETYPE.CONTROLLER) return 10;
        if (apiType == APIRESOURCETYPE.COMPLEXTYPE) return 5;
        if (apiType == APIRESOURCETYPE.ENUMERABLE) return 1;
        return 0;
    }
    //#endregion

    var createViewModel = function (data) {
        var viewModel = new ViewModel();

        data.ResouceModels.forEach(function (resourceModel) {
            viewModel.apiDescriptions.push(
                new ApiDescription(resourceModel.Name, resourceModel.Description, resourceModel.Url, resourceModel.Type)
            );
        });

        viewModel.apiDescriptions.sort(function (l, r) {
            // sort by type
            var lTypeImportance = apiTypeOrderWeight(l.apiType);
            var rTypeImportance = apiTypeOrderWeight(r.apiType);
            // then by name
            var nameImportance = l.name == r.name ? 0 : (l.name < r.name ? -1 : 1);

            return (rTypeImportance - lTypeImportance) + nameImportance;
        });

        // load default configurations
        var controllerSettings = retreaveSetting("ShowControllers");
        if (controllerSettings == null)
            viewModel._showControllers(true);
        else
            viewModel._showControllers(controllerSettings === 'true');
        viewModel._showModels(retreaveSetting("ShowModels") === 'true');

        // initial filter
        viewModel.searchApiData();

        return viewModel;
    };

    var container = document.getElementById('dashboard');
    var containerSelector = $(container).data('url');

    $.ajax({
        url: containerSelector,
        dataType: "json"
    }).done(function (data) {
        var viewModel = createViewModel(data);
        ko.applyBindings(viewModel);
    });
})();