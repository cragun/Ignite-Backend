(function () {
    //#region UI manipulation logic
    var scrollToElement = function (element) {
        var offset = element.offset().top - $(window).scrollTop();

        if (offset > window.innerHeight) {
            // not in view so scroll to it
            $('html, body').animate({ scrollTop: offset }, 1000);
        }
    };

    var displayContentRegion = function (regionId) {
        var element = $(regionId);
        if (!element)
            return;

        var parent = element.parent();

        if (element.hasClass('hidden')) {
            // element was invisible => display
            // hard coded for parent of type panel-body that can also be hidden, check if parent is visible
            if (parent.hasClass('hidden')) {
                parent.removeClass('hidden');
            } else {
                element.siblings().addClass('hidden');
            }
            element.removeClass('hidden');
            scrollToElement(parent.closest('.panel'));
        } else {
            // element was visible => hide
            element.addClass('hidden');
            parent.addClass('hidden');
        }
    };
    //#endregion

    var loadedApiInteractions = {};

    var createViewInteractionModel = function (data) {
        var parameters = [];
        data.UriParameters.forEach(function (uriParam) {
            parameters.push(new DayCare.HelpPage.Interaction.InteractionParameterModel(uriParam.Name, uriParam.TypeDescription.Name, uriParam.DefaultValue));
        });

        var sampleRequest = null, sampleResponse = null, actionParameters = {
            url: data.ActionUrl,
            httpVerbe: data.HttpActionVerb
        };

        if (data.SampleRequests != null)
            sampleRequest = data.SampleRequests.Text;

        if (data.SampleResponses != null)
            sampleResponse = data.SampleResponses.Text;

        return new DayCare.HelpPage.Interaction.InteractionViewModel(parameters, sampleRequest, sampleResponse, actionParameters);
    };

    var handleProcessUpdate = function (url, regionId) {
        if (loadedApiInteractions[regionId])
            return;

        $.ajax({
            url: url,
            dataType: "json"
        }).done(function (data) {
            if (loadedApiInteractions[regionId])
                return;
            else
                loadedApiInteractions[regionId] = createViewInteractionModel(data);

            DayCare.HelpPage.Interaction.initializeInteractionContainer(regionId, loadedApiInteractions[regionId]);
        });
    };

    //#region initialization
    $('a[data-function]').click(function () {
        var a = $(this);
        var toglerdContainer = a.attr('href');
        var functionTypes = a.data('function').replace(/[\s,]+/g, ',').split(',');

        if (functionTypes.indexOf("load-api-interaction") != -1) {
            var url = a.data('apidataurl');
            handleProcessUpdate(url, toglerdContainer.substr(1));
        }

        if (functionTypes.indexOf("togle-display") != -1) {
            displayContentRegion(toglerdContainer);
        }
    });

    if (window.location.hash != '') {
        var actionContainer = window.location.hash.substr(1);
        var hyperlinkDomObject = $('a[href$="' + actionContainer + '"]');

        displayContentRegion("#" + actionContainer);
        handleProcessUpdate(hyperlinkDomObject.data('apidataurl'), actionContainer);
    }
    //#endregion
})();