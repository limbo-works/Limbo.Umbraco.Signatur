angular.module("umbraco").controller("Limbo.Umbraco.Signatur.JobData.Controller", function ($scope) {

    const vm = this;

    if (!$scope.model.value) return;

    const parser = new DOMParser();

    const doc = parser.parseFromString($scope.model.value, "application/xml");

    const parseError = doc.querySelector("parsererror");

    if (parseError) {
        vm.error = "Failed parsing XML value.";
        return;
    }

    vm.job = {
        id: doc.querySelector("webAdId")?.innerHTML,
        title: doc.querySelector("title")?.innerHTML,
        category: doc.querySelector("category")?.innerHTML,
        applicationUrl: doc.querySelector("applicationUrl")?.innerHTML,
        pubDate: parseDate(doc.querySelector("pubDate")?.innerHTML),
        expDate: parseDate(doc.querySelector("expDate")?.innerHTML),
        deadline: doc.querySelector("deadline")?.innerHTML,
        categories: doc.querySelector("categories")?.innerHTML.split("; ") ?? []
    };

    function parseDate(value) {
        if (!value) return null;
        const date = new Date(value);
        return {
            date: date,
            comment: moment().to(date)
        };
    }

});