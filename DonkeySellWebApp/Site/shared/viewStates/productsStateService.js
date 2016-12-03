app.service('productsStateService', [productsStateService]);

function productsStateService() {
    this.set = function(state) {
        this.state = state;
    }

    this.get = function() {
        return this.state;
    }

    this.hasState = function () {
        return this.state !== undefined;
    }

    return this;
};
