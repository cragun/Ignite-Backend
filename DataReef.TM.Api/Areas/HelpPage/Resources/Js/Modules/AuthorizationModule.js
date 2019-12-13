if (typeof DayCare.HelpPage.Options === 'undefined') {
    throw new Error('Options module is required for Authorization module to work');
}

DayCare.HelpPage = DayCare.HelpPage || {};
DayCare.HelpPage.AuthorizationModule = (function (my) {
    my.startFunction = function () {

    };

    return my;
}(DayCare.HelpPage.AuthorizationModule || {}));