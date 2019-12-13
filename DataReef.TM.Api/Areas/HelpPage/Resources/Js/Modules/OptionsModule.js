if (typeof DayCare.HelpPage.Persistence === 'undefined') {
    throw new Error('Persistence module is required');
}

DayCare.HelpPage.Options = (function (my) {
    var persistedHeaderValuesPrefix = "HeaderValues-";
    var authTokenPersistanceKey = persistedHeaderValuesPrefix + "AuthToken";
    var filteredCustomHeaderParameterName = [authTokenPersistanceKey];

    var persistance = DayCare.HelpPage.Persistence;

    function HeaderDescription(name, value) {
        this.name = name;
        this.value = value;
    }

    my.HeaderDescription = HeaderDescription;

    my.saveAuthToken = function (authToken) {
        persistance.persist(authTokenPersistanceKey, authToken);
    }

    my.getAuthToken = function () {
        return persistance.retrieve(authTokenPersistanceKey);
    }

    my.getAdditionalHeaderDescriptions = function () {
        var headerDescriptions = [];
        persistance.retrieveAll().forEach(function (kvp) {
            // filter out any other type of kvp that does not start with the required prefix
            if (kvp.name.indexOf(persistedHeaderValuesPrefix) != 0) return;

            // filter out specific header values like AuthToken
            if (filteredCustomHeaderParameterName.indexOf(kvp.name) != -1) return;

            var headerName = kvp.name.slice(kvp.name.indexOf(persistedHeaderValuesPrefix) + persistedHeaderValuesPrefix.length);
            headerDescriptions.push(new HeaderDescription(headerName, kvp.value));
        });
        return headerDescriptions;
    }

    my.removeAdditionalHeaderDescription = function (headerName) {
        persistance.remove(persistedHeaderValuesPrefix + headerName);
    }

    my.saveAdditionalHeaderDescriptions = function (headerDescriptions) {
        headerDescriptions.forEach(function (headerDescription) {
            my.saveHeaderParameter(headerDescription);
        });
    }

    my.saveAdditionalHeaderDescription = function (headerDescription) {
        if (headerDescription instanceof HeaderDescription) {
            persistance.persist(persistedHeaderValuesPrefix + headerDescription.name, headerDescription.value);
        } else {
            console.warn("Failed to save header parameter due to type mismatch, parameter must be of type HeaderDescription");
        }
    }

    return my;
}(DayCare.HelpPage.Options || {}));