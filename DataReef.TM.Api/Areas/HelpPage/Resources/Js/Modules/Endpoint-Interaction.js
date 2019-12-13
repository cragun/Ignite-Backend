if (typeof ko === 'undefined') {
    throw new Error('Knockout is required');
}

DayCare.HelpPage = DayCare.HelpPage || {};
DayCare.HelpPage.Interaction = (function (my) {
    var buildCompleteUrl = function (url, parameters) {
        if (parameters.length <= 0)
            return url;

        var tokenized = url;
        var removeUrlParameters = [];

        // fill in the url tokens
        parameters.forEach(function (parameter) {
            var value = parameter.value();
            if (value == null || value == '')
                removeUrlParameters.push(parameter.name);
            else
                tokenized = tokenized.replace('{' + parameter.name + '}', value);
        });

        if (removeUrlParameters.length == 0)
            return tokenized;

        // remove empty parameters
        var urlparts = tokenized.split('?');
        if (urlparts.length >= 2) {
            var pars = urlparts[1].split(/[&;]/g);

            for (var j = 0; j < removeUrlParameters.length; j++) {
                var currentParameter = removeUrlParameters[j];
                var prefix = encodeURIComponent(currentParameter) + '=';

                //reverse iteration as may be destructive
                for (var i = pars.length; i-- > 0;) {
                    //idiom for string.startsWith
                    if (pars[i].lastIndexOf(prefix, 0) !== -1) {
                        pars.splice(i, 1);
                    }
                }
            }

            tokenized = urlparts[0] + '?' + pars.join('&');
        }

        return tokenized;
    }

    var assembleRequestHeaders = function () {
        var requestHeaders = {};

        DayCare.HelpPage.Options.getAdditionalHeaderDescriptions().forEach(function (additionalHeader) {
            requestHeaders[additionalHeader.name] = additionalHeader.value;
        });

        var authToken = DayCare.HelpPage.Options.getAuthToken();
        if (authToken)
            requestHeaders["Authorization"] = authToken;

        return requestHeaders;
    }

    // TODO [Mihai] I will need to move the clipboard request builder to another javascript file
    var assembleRequestCall = function (tool, url, verbe, bodyData, headers) {
        switch (tool) {
            case 'CURL':
                // HTTP verb
                var curlRequest = 'curl -H \"Content-Type:application/json\"  -X ' + verbe + ' ';

                // message headers
                if (headers)
                    Object.keys(headers).forEach(function (key) {
                        curlRequest += '-H "' + key + ': ' + headers[key] + '" ';
                    });

                // message body
                if (bodyData && bodyData.length > 0) {
                    bodyData = bodyData.replace(/\"/g, '\\\"');
                    curlRequest += '-d "' + bodyData + '" ';
                }

                // message URL
                curlRequest += url;

                // remove all new line separators
                curlRequest = curlRequest.replace(/(?:\r\n|\r|\n)/g, '');

                return curlRequest;
            case 'URL':
                var urlRequest = url;

                return urlRequest;
            default:
                return 'Unsupported tool';
        }
    }

    // TODO [Mihai] I will need to move interaction request handler to another javascript file
    var interactionResultSuccessModal = document.getElementById("intractionInvoke-success-modal");
    var interactionResultErrorModal = document.getElementById("intractionInvoke-error-alert");
    var invokeRequest = function (viewModel) {
        var processedUrl = buildCompleteUrl(viewModel.actionURL, viewModel.parameters());
        var requestHeaders = assembleRequestHeaders();

        var additionalHeadersArray = [];
        Object.keys(requestHeaders).forEach(function (key) {
            additionalHeadersArray.push(key + ': ' + requestHeaders[key]);
        });

        $.ajax({
            url: processedUrl,
            type: viewModel.httpVerbe,
            contentType: "application/json",
            headers: requestHeaders,
            data: viewModel.body(),
            success: function (data, textStatus, xhr) {
                var responseVm = {
                    requestUrl: processedUrl,
                    requestStatusCode: xhr.status,
                    requestHeaders: additionalHeadersArray,
                    responseHeaders: xhr.getAllResponseHeaders().split(/\n/),
                    responseData: (typeof data === 'undefined' || data == null || (typeof data === 'string' && data.trim() === '')) ?
                        null :
                        JSON.stringify(data, null, "\t")
                }

                ko.cleanNode(interactionResultSuccessModal);
                ko.applyBindings(responseVm, interactionResultSuccessModal);
                $(interactionResultSuccessModal).modal('show');

                viewModel.invokingAction(false);
            },
            error: function (xhr, textStatus, errorThrown) {
                var alertVm = {
                    requestUrl: processedUrl,
                    requestStatusCode: xhr.status,
                    requestHeaders: additionalHeadersArray,
                    requestError: textStatus + ", " + errorThrown,
                    errorDescription: xhr.responseText
                }
                ko.cleanNode(interactionResultErrorModal);
                ko.applyBindings(alertVm, interactionResultErrorModal);
                $(interactionResultErrorModal).modal('show');

                viewModel.invokingAction(false);
            }
        });
    }

    var mapRequestBuilderTools = function (viewModel, container) {
        $(container).find('[data-requesttoclipboard]').each(function (index, domElement) {
            var domElementSelector = $(domElement);
            var tool = domElementSelector.data('requesttoclipboard');

            var clip = DayCare.ZeroClipboard.createClipboard(domElementSelector);

            clip.on("copy", function (clipboardEvent) {
                var clipboard = clipboardEvent.clipboardData;

                var processedUrl = buildCompleteUrl(viewModel.actionURL, viewModel.parameters());
                var requestHeaders = assembleRequestHeaders();

                var assembledRequest = assembleRequestCall(tool, processedUrl, viewModel.httpVerbe, viewModel.body(), requestHeaders);

                clipboard.setData("text/plain", assembledRequest);
            });
        });
    }

    function InteractionParameterModel(name, type, value) {
        this.name = name;
        this.type = type;
        this.value = ko.observable(value);
    }

    function InteractionViewModel(parameters, body, expectedResponse, actionParameters) {
        var self = this;

        // public properties
        self.isLoaded = ko.observable(true);
        self.parameters = ko.observableArray([]);
        self.body = ko.observable();
        self.expectedResponse = expectedResponse;
        self.actionURL = actionParameters.url;
        self.httpVerbe = actionParameters.httpVerbe;
        self.invokingAction = ko.observable(false);

        // computed properties
        self.hasParameters = ko.pureComputed(function () {
            return self.parameters().length > 0;
        }, this);
        self.hasBody = ko.pureComputed(function () {
            return self.body() && self.body().length > 0;
        }, this);
        self.hasResponse = ko.pureComputed(function () {
            return self.expectedResponse != null && self.expectedResponse.length > 0;
        }, this);

        // commands
        self.invokeApi = function () {
            self.invokingAction(true);
            invokeRequest(self);
        }

        // constructor stuff
        if (parameters && parameters instanceof Array)
            parameters.forEach(function (param) {
                if (param instanceof InteractionParameterModel)
                    self.parameters.push(param);
            });

        if (body) self.body(body);
    };

    my.InteractionViewModel = InteractionViewModel;
    my.InteractionParameterModel = InteractionParameterModel;

    my.initializeInteractionContainer = function (containerId, viewModel) {
        if (!containerId || !viewModel) return;
        if (!(viewModel instanceof InteractionViewModel)) throw new Error("viewModel is not of type InteractionViewModel");

        var container = document.getElementById(containerId);

        if (!!ko.dataFor(container))
            return;

        ko.applyBindings(viewModel, container);
        mapRequestBuilderTools(viewModel, container);
    };

    return my;
}(DayCare.HelpPage.Interaction || {}));