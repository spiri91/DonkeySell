app.service('userStateService', [userStateService]);

function userStateService() {
    this.set = function (state) {
        this.state = state;
    }

    this.get = function () {
        return this.state;
    }

    this.hasState = function() {
        return this.state !== undefined;
    }

    this.getUsernameFromState = function() {
        if (this.state)
            return this.state.username;

        return null;
    }

    return this;
};
