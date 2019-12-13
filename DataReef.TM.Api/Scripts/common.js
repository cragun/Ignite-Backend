var DayCare = DayCare || {
    ZeroClipboard: {
        isInitialized: false,
        createClipboard: function (element) {
            if (!this.isInitialized) {
                ZeroClipboard.config({ swfPath: "/Scripts/ZeroClipboard.swf" });
                this.isInitialized = true;
            }

            if (typeof element === "string")
                return new ZeroClipboard(document.getElementById(element));

            return new ZeroClipboard(element);
        }
    }
};