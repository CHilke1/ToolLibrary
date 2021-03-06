﻿// Tool model
function ToolModel(data) {
    var self = this;
    self.Name = data.Name;
    self.Description = data.Description;
    self.Manufacturer = data.Manufacturer;
    self.ImageUrl = data.ImageUrl;
    self.Category = data.Category;
    self.Type = data.Type;
}

function ExtractModels(parent, data, constructor) {
    var models = [];
    for (var index = 0; index < data.length; index++) {
        var row = data[index];
        var model = new constructor(row, parent);
        models.push(model);
    }
    return models;
}

function GetObservableArray(array) {
    if (typeof (array) == 'function') {
        return array;
    }
    return ko.observableArray(array);
}

function CustomerPageModel(data) {
    if (!data) {
        data = {};
    }

    var self = this;
    self.customers = ExtractModels(self, data.customers, ToolModel);

    var filters = [];
    var sortOptions = [];
    self.filter = new FilterModel(filters, self.customers);
    self.sorter = new SorterModel(sortOptions, self.filter.filteredRecords);
    self.pager = new PagerModel(self.sorter.orderedRecords);
}

function FilterModel(filters, records) {
    var self = this;
    self.records = GetObservableArray(records);
    self.filters = ko.observableArray(filters);
    self.activeFilters = ko.computed(function () {
        var filters = self.filters();
        var activeFilters = [];
        for (var index = 0; index < filters.length; index++) {
            var filter = filters[index];
            if (filter.CurrentOption) {
                var filterOption = filter.CurrentOption();
                if (filterOption && filterOption.FilterValue != null) {
                    var activeFilter = {
                        Filter: filter,
                        IsFiltered: function (filter, record) {
                            var filterOption = filter.CurrentOption();
                            if (!filterOption) {
                                return;
                            }

                            var recordValue = filter.RecordValue(record);
                            return recordValue != filterOption.FilterValue; NoMat
                        }
                    };
                    activeFilters.push(activeFilter);
                }
            }
            else if (filter.Value) {
                var filterValue = filter.Value();
                if (filterValue && filterValue != "") {
                    var activeFilter = {
                        Filter: filter,
                        IsFiltered: function (filter, record) {
                            var filterValue = filter.Value();
                            filterValue = filterValue.toUpperCase();

                            var recordValue = filter.RecordValue(record);
                            recordValue = recordValue.toUpperCase();
                            return recordValue.indexOf(filterValue) == -1;
                        }
                    };
                    activeFilters.push(activeFilter);
                }
            }
        }

        return activeFilters;
    });
    self.filteredRecords = ko.computed(function () {
        var records = self.records();
        var filters = self.activeFilters();
        if (filters.length == 0) {
            return records;
        }

        var filteredRecords = [];
        for (var rIndex = 0; rIndex < records.length; rIndex++) {
            var isIncluded = true;
            var record = records[rIndex];
            for (var fIndex = 0; fIndex < filters.length; fIndex++) {
                var filter = filters[fIndex];
                var isFiltered = filter.IsFiltered(filter.Filter, record);
                if (isFiltered) {
                    isIncluded = false;
                    break;
                }
            }

            if (isIncluded) {
                filteredRecords.push(record);
            }
        }

        return filteredRecords;
    }).extend({ throttle: 200 });
}

function PagerModel(records) {
    var self = this;
    self.pageSizeOptions = ko.observableArray([1, 5, 25, 50, 100, 250, 500]);

    self.records = GetObservableArray(records);
    self.currentPageIndex = ko.observable(self.records().length > 0 ? 0 : -1);
    self.currentPageSize = ko.observable(25);
    self.recordCount = ko.computed(function () {
        return self.records().length;
    });
    self.maxPageIndex = ko.computed(function () {
        return Math.ceil(self.records().length / self.currentPageSize()) - 1;
    });
    self.currentPageRecords = ko.computed(function () {
        var newPageIndex = -1;
        var pageIndex = self.currentPageIndex();
        var maxPageIndex = self.maxPageIndex();
        if (pageIndex > maxPageIndex) {
            newPageIndex = maxPageIndex;
        }
        else if (pageIndex == -1) {
            if (maxPageIndex > -1) {
                newPageIndex = 0;
            }
            else {
                newPageIndex = -2;
            }
        }
        else {
            newPageIndex = pageIndex;
        }

        if (newPageIndex != pageIndex) {
            if (newPageIndex >= -1) {
                self.currentPageIndex(newPageIndex);
            }

            return [];
        }

        var pageSize = self.currentPageSize();
        var startIndex = pageIndex * pageSize;
        var endIndex = startIndex + pageSize;
        return self.records().slice(startIndex, endIndex);
    }).extend({ throttle: 5 });
    self.moveFirst = function () {
        self.changePageIndex(0);
    };
    self.movePrevious = function () {
        self.changePageIndex(self.currentPageIndex() - 1);
    };
    self.moveNext = function () {
        self.changePageIndex(self.currentPageIndex() + 1);
    };
    self.moveLast = function () {
        self.changePageIndex(self.maxPageIndex());
    };
    self.changePageIndex = function (newIndex) {
        if (newIndex < 0
			|| newIndex == self.currentPageIndex()
			|| newIndex > self.maxPageIndex()) {
            return;
        }

        self.currentPageIndex(newIndex);
    };
    self.onPageSizeChange = function () {
        self.currentPageIndex(0);
    };
    self.renderPagers = function () {
        var pager = "<div><a href=\"#\" data-bind=\"click: pager.moveFirst, enable: pager.currentPageIndex() > 0\">&lt;&lt;</a><a href=\"#\" data-bind=\"click: pager.movePrevious, enable: pager.currentPageIndex() > 0\">&lt;</a>Page <span data-bind=\"text: pager.currentPageIndex() + 1\"></span> of <span data-bind=\"text: pager.maxPageIndex() + 1\"></span> [<span data-bind=\"text: pager.recordCount\"></span> Record(s)]<select data-bind=\"options: pager.pageSizeOptions, value: pager.currentPageSize, event: { change: pager.onPageSizeChange }\"></select><a href=\"#\" data-bind=\"click: pager.moveNext, enable: pager.currentPageIndex() < pager.maxPageIndex()\">&gt;</a><a href=\"#\" data-bind=\"click: pager.moveLast, enable: pager.currentPageIndex() < pager.maxPageIndex()\">&gt;&gt;</a></div>";
        $("div.Pager").html(pager);
    };
    self.renderNoRecords = function () {
        var message = "<span data-bind=\"visible: pager.recordCount() == 0\">No records found.</span>";
        $("div.NoRecords").html(message);
    };

    self.renderPagers();
    self.renderNoRecords();
}

function SorterModel(sortOptions, records) {
    var self = this;
    self.records = GetObservableArray(records);
    self.sortOptions = ko.observableArray(sortOptions);
    self.sortDirections = ko.observableArray([
		{
		    Name: "Asc",
		    Value: "Asc",
		    Sort: false
		},
		{
		    Name: "Desc",
		    Value: "Desc",
		    Sort: true
		}]);
    self.currentSortOption = ko.observable(self.sortOptions()[0]);
    self.currentSortDirection = ko.observable(self.sortDirections()[0]);
    self.orderedRecords = ko.computed(function () {
        var records = self.records();
        var sortOption = self.currentSortOption();
        var sortDirection = self.currentSortDirection();
        if (sortOption == null || sortDirection == null) {
            return records;
        }

        var sortedRecords = records.slice(0, records.length);
        SortArray(sortedRecords, sortDirection.Sort, sortOption.Sort);
        return sortedRecords;
    }).extend({ throttle: 5 });
}

function FilterModel(filters, records) {
    var self = this;
    self.records = GetObservableArray(records);
    self.filters = ko.observableArray(filters);
    self.activeFilters = ko.computed(function () {
        var filters = self.filters();
        var activeFilters = [];
        for (var index = 0; index < filters.length; index++) {
            var filter = filters[index];
            if (filter.CurrentOption) {
                var filterOption = filter.CurrentOption();
                if (filterOption && filterOption.FilterValue != null) {
                    var activeFilter = {
                        Filter: filter,
                        IsFiltered: function (filter, record) {
                            var filterOption = filter.CurrentOption();
                            if (!filterOption) {
                                return;
                            }

                            var recordValue = filter.RecordValue(record);
                            return recordValue != filterOption.FilterValue; NoMat
                        }
                    };
                    activeFilters.push(activeFilter);
                }
            }
            else if (filter.Value) {
                var filterValue = filter.Value();
                if (filterValue && filterValue != "") {
                    var activeFilter = {
                        Filter: filter,
                        IsFiltered: function (filter, record) {
                            var filterValue = filter.Value();
                            filterValue = filterValue.toUpperCase();

                            var recordValue = filter.RecordValue(record);
                            recordValue = recordValue.toUpperCase();
                            return recordValue.indexOf(filterValue) == -1;
                        }
                    };
                    activeFilters.push(activeFilter);
                }
            }
        }

        return activeFilters;
    });
    self.filteredRecords = ko.computed(function () {
        var records = self.records();
        var filters = self.activeFilters();
        if (filters.length == 0) {
            return records;
        }

        var filteredRecords = [];
        for (var rIndex = 0; rIndex < records.length; rIndex++) {
            var isIncluded = true;
            var record = records[rIndex];
            for (var fIndex = 0; fIndex < filters.length; fIndex++) {
                var filter = filters[fIndex];
                var isFiltered = filter.IsFiltered(filter.Filter, record);
                if (isFiltered) {
                    isIncluded = false;
                    break;
                }
            }

            if (isIncluded) {
                filteredRecords.push(record);
            }
        }

        return filteredRecords;
    }).extend({ throttle: 200 });
}
//test values
var testTools = [{
    Name: "1",
    Description: "Test 1",
    Manufacturer: "None",
    ImageUrl: "xxx",
    Type: "yyy"
},
{
    Name: "2",
    Description: "Another Customer",
    Manufacturer: "None",
    ImageUrl: "xxx",
    Type: "yyy"
},
{
    Name: "3",
    Description: "Third Largest",
    Manufacturer: "None",
    ImageUrl: "xxx",
    Type: "yyy"
},
{
    Name: "4",
    Description: "Fourth in Line",
    Manufacturer: "None",
    ImageUrl: "xxx",
    Type: "yyy"
}];

var testData = {
    tools: testTools
};

ko.applyBindings(new CustomerPageModel(testData));
//http://jsfiddle.net/JTimperley/pyCTN/13/
//http://geekswithblogs.net/jtimperley/archive/2013/07/29/knockout.js---filtering-sorting-and-paging.aspx