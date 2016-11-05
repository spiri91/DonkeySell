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
