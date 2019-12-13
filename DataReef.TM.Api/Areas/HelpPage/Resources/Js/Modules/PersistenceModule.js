DayCare.HelpPage = DayCare.HelpPage || {};
DayCare.HelpPage.Persistence = (function (my) {
    my.retrieve = function (name) {
        return localStorage.getItem(name);
    };

    my.retrieveAll = function () {
        var kvps = [];

        for (var i = 0, len = localStorage.length; i < len; i++) {
            var key = localStorage.key(i);
            var value = localStorage[key];

            kvps.push({ name: key, value: value });
        }

        return kvps;
    };

    my.persist = function (name, value) {
        localStorage.setItem(name, value);
    };

    my.remove = function (name) {
        localStorage.removeItem(name);
    }

    return my;
}(DayCare.HelpPage.Persistence || {}));