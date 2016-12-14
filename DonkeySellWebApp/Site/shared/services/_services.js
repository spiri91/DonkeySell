app.service('favoritesService', ['apiRootAddress', '$http', favoritesService]);

function favoritesService(apiRootAddress, $http) {
    this.composeAddress = function (username) {
        return apiRootAddress + "Users/" + username + "/Favorites";
    }

    this.getFavoritesProducts = function(username, token) {
        return $http({
            method: 'GET',
            url: this.composeAddress(username),
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.postProductToFavorites = function(username, token, productId) {
        return $http({
            method: 'POST',
            url: this.composeAddress(username) + '/' + productId,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.deleteFromFavoritesProducts = function(username, token, productId) {
        return $http({
            method: 'DELETE',
            url: this.composeAddress(username) + '/' + productId,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    return this;
}
app.service('friendsService', ['apiRootAddress', '$http', friendsService]);

function friendsService(apiRootAddress, $http) {
    this.composeAddress = function (username) {
        return apiRootAddress + "Users/" + username + "/Friends";
    }

    this.getFriends = function(username, token) {
        return $http({
            method: 'GET',
            url: this.composeAddress(username),
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.postFriend = function(username, friend, token) {
        return $http({
            method: 'POST',
            url: this.composeAddress(username) + "/" + friend,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.deleteFriend = function(username, friend, token) {
        return $http({
            method: 'DELETE',
            url: this.composeAddress(username) + "/" + friend,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    return this;
};
app.service('improvementsService', ['apiRootAddress', '$http', improvementsService]);

function improvementsService(apiRootAddress, $http) {
    this.fullApiAddress = apiRootAddress + "Improvements";

    this.postImprovement =function(improvement) {
        return $http.post(this.fullApiAddress, improvement);
    }

    return this;
};
app.service('messagesService', ['apiRootAddress', '$http', messagesService]);

function messagesService(apiRootAddress, $http) {
    this.composeAddress = function(id) {
        return apiRootAddress+"Products/" + id + "/Messages"; 
    }

    this.getMessagesForProduct = function(id) {
        let fullApiAddress = this.composeAddress(id);

        return $http.get(fullApiAddress);
    }

    this.postMessageForProduct = function (message, token) {
        let fullApiAddress = this.composeAddress(message.productId);

        return $http({
            method: 'POST',
            url: fullApiAddress,
            headers: { 'Authorization': 'Bearer ' + token },
            data: message
        });
    }

    this.deleteMessage = function (productId, messageId, token) {
        let fullApiAddress = this.composeAddress(productId) + "/" + messageId;

        return $http({ method: 'DELETE', url: fullApiAddress, headers: { 'Authorization': 'Bearer ' + token } });
    }

    return this;
}
app.service('othersService', ['apiRootAddress', '$http', othersService]);

function othersService(apiRootAddress, $http) {
    this.urlApiAddress = apiRootAddress + "Other";

    this.getCategories = function() {
        let urlAddress = this.urlApiAddress + "/categories";

        return $http.get(urlAddress);
    }

    this.getCities = function() {
        let urlAddress = this.urlApiAddress + "/cities";

        return $http.get(urlAddress);
    }

    this.usernameIsTaken = function(username) {
        let urlAddress = this.urlApiAddress + "/usernameIsNotTaken/" + username;

        return $http.get(urlAddress);
    }

    this.emailInUse = function (email) {
        let emailWithoutDots = email.replace(/\./g, '$');
        let urlAddress = this.urlApiAddress + "/emailNotInUse/" + emailWithoutDots;

        return $http.get(urlAddress);
    };

    this.usersLike = function(username) {
        let urlAddress = this.urlApiAddress + "/findUser/" + username;

        return $http.get(urlAddress);
    }

    return this;
}
app.service('productsService', ['apiRootAddress', '$http', productsService]);

function productsService(apiRootAddress, $http) {
    this.productsApiAdress = apiRootAddress + "Products";

    this.queryProducts = function (query, take, skip, orderBy, sortDirection) {
        return $http({
            method: 'GET',
            url: this.productsApiAdress +
                "/query/" +
                query +
                "/" +
                take +
                "/" +
                skip +
                "/" +
                orderBy +
                "/" +
                sortDirection
        });
    }

    this.getProduct = function (id) {
        return $http.get(this.productsApiAdress + "/" + id);
    }

    this.deleteProduct = function (id, token) {
        return $http({
            method: 'DELETE',
            url: this.productsApiAdress + "/" + id,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.addProduct = function (product, token) {
        return $http({
            method: 'POST',
            url: this.productsApiAdress,
            headers: { 'Authorization': 'Bearer ' + token },
            data: product
        });
    }

    this.getProductsOfUser = function(username) {
        return $http.get(this.productsApiAdress + "/" + username);
    }

    return this;
}

app.service('productNavigationService', ['productsService', '$q', productNavigationService]);

function productNavigationService(productsService, $q) {
    this.query = "";
    this.skip = 0;
    this.sortBy = {};
    this.currentId = 0;
    this.productsIds = [];
    this.count = 0;
    this.defaultItemsPerPage = 10;
    this.productsService = productsService;

    this.setValues = function(query, skip, sortBy, count, productIds) {
        this.query = query;
        this.skip = skip;
        this.sortBy = sortBy;
        this.count = count;
        this.productsIds = productIds;
    };

    this.getNextProduct = function(id) {
        let currentIndex = this.productsIds.indexOf(id);

        if (currentIndex === this.productsIds.length - 1)
            return null;

        return this.productsIds[currentIndex + 1];
    };

    this.getPreviousProduct = function (id) {
        let currentIndex = this.productsIds.indexOf(id);
        if (currentIndex === 0)
            return null;

        return this.productsIds[currentIndex - 1];
    };

    this.getNextProducts = function () {
        this.skip += this.productsIds.length;
        return this.getProducts();
    };

    this.getPreviousProducts = function () {
        this.skip -= this.defaultItemsPerPage;
        if (this.skip < 0)
            this.skip = 0;

        return this.getProducts();
    };

    this.getProducts = function () {
        if (this.query)
            return this.productsService.queryProducts(this.query,
                this.defaultItemsPerPage,
                this.skip,
                this.sortBy.value,
                this.sortBy.sortDirection);
        else {
            let deferred = $q.defer();
            deferred.resolve();
            return deferred.promise;
        }
    };

    this.setProductsIds = function (ids, inFront) {
        for (let i = 0; i <= ids.length; ++i)
            inFront === false ? this.productsIds.push(ids[i]) : this.productsIds.unshift(ids[i]);
    }

    this.checkIfIdIsAllreadyAdded = function(id) {
        return this.productsIds.indexOf(id) >= 0;
    };

    return this;
};
app.service('queryBuilderService', [queryBuilderService]);

function queryBuilderService() {
    this.buildQuery = function(queryParts) {
        let query = "";
        for (let i = 0; i < queryParts.length; i++) {
            let part = queryParts[i].title + ',' + queryParts[i].minValue;
            if (queryParts[i].maxValue)
                part += ',' + queryParts[i].maxValue;
            if (queryParts[i].useLike)
                part += ',' + 'true';
            if (queryParts[i].useOr)
                part += ',' + 'true';

            if (i === 0) 
                query += part;
            else
                query += ";" + part;
        }

        return query;
    }

    return this;
}

app.service('sortOptionsService', [sortOptionsService]);

function sortOptionsService() {
    this.getSortOptions = function() {
        let sortOptions = [];

        sortOptions.push(new SortOption(1, "Price desc", "Price", sortDirection.descending));
        sortOptions.push(new SortOption(2, "Price asc", "Price", sortDirection.ascending));

        sortOptions.push(new SortOption(3, "Date desc", "DatePublished", sortDirection.descending));
        sortOptions.push(new SortOption(4, "Date asc", "DatePublished", sortDirection.ascending));

        sortOptions.push(new SortOption(5, "User desc", "UserName", sortDirection.descending));
        sortOptions.push(new SortOption(6, "User asc", "UserName", sortDirection.ascending));

        return sortOptions;
    }

    return this;
}
app.service('storageService', [storageService]);

function storageService() {
    this.set = function(key, value) {
        localStorage.setItem(key, value);
    };

    this.get = function(key) {
        return localStorage.getItem(key);
    };

    this.remove = function(key) {
        localStorage.removeItem(key);
    };

    return this;
}
app.service('usersService', ['apiRootAddress', '$http', 'storageService', usersService]);

function usersService(apiRootAddress, $http, storageService) {
    this.tokenApiAddress = apiRootAddress.substring(0, apiRootAddress.length - 4) + "Token";
    this.usersApiAdress = apiRootAddress + "Users";

    this.logIn = function (user) {
        let getTokenDataString = "grant_type=password&username=" + user.userName + "&password=" + user.password;

        return $.ajax({
            method: 'POST',
            url: this.tokenApiAddress,
            headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
            data: getTokenDataString
        }).then(function (data) {
            storageService.set('token', data.access_token);
            return true;
        }).fail(
                function (error) {
                    console.log(error.message);
                    return false;
                });
    };

    this.signOut = function () {
        storageService.remove('token');
        return true;
    }

    this.getUser = function (username) {
        let getUserAddress = this.usersApiAdress + "/" + username;

        return $http({ method: 'GET', url: getUserAddress });
    }

    this.createEditUser = function (user) {
        return $http.post(this.usersApiAdress, user);
    }

    this.deleteUser = function (username, token) {
        return $http({
            method: 'DELETE',
            url: this.usersApiAdress + "/" + username,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.getUnreadMessages = function (username, token) {
        return $http({
            method: 'GET',
            url: this.usersApiAdress + "/" + username + "/unread",
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.markMessageRead = function (username, token, messageId) {
        return $http({
            method: 'POST',
            url: this.usersApiAdress + "/" + username + "/MarkRead/" + messageId,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.changePassword = function (username, resetPassword) {
        return $http.post(this.usersApiAdress + "/" + username + "/changePassword", resetPassword);
    }

    this.resetPassword = function (username) {
        return $http.post(this.usersApiAdress + "/" + username + "/resetPassword");
    }

    this.getToken = function () {
        return storageService.get('token');
    }

    return this;
}