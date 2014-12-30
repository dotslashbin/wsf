

    // 03.29.14 require knockout.mapping
    //
    //
    //


function PageData() {

    var self = this;

    self.treeItem = null;
    self.treeItemArray = ko.observableArray();

    self.drillDown = function (view, model) {
        self.drillDownExt(view, model, "unknown");
    };


    self.drillDownExt = function (view, model,name) {
        var nextTreeItem = {
            previous: self.treeItem,
            viewname: view,
            model: model,
            name : name
        }
        self.treeItem = nextTreeItem;
        self.updateViewModel();
    };




    self.drillUp = function () {
        if (self.treeItem.previous != null) {
            self.treeItem = self.treeItem.previous;
            self.updateViewModel();
        }
    };


    self.drillDownTo = function(item){
        self.treeItem = item;
        self.updateViewModel();
    }



    self.mvInit = { view: null, model: null };
    self.mv = ko.observable(self.mvInit);


    self.modelInit = function (elements) {
        if (self.mv() != null && self.mv().model != null && self.mv().model.init !== undefined) {
            self.mv().model.init(elements);
        } else {

        }
    }


    self.updateViewModel = function () {

        self.mv({ model: self.treeItem.model, view: self.treeItem.viewname });

        var items = [];
        var cursor = self.treeItem;

        while (cursor != null) {
            items.unshift(cursor);
            cursor = cursor.previous;
        }

        self.treeItemArray(items);

    }



    self.dmvInit = { view: null, model: null, callback: null };
    self.dmv = ko.observable(self.dmvInit);


    self.dialogInit = function (elements) {
        if (self.dmv().model  && self.dmv().model.init ) {
            self.dmv().model.init(elements);
        }
        else {
        }
    }


    self.OpenDialogWithName = function (dlgName, model, callback, view) {

        self.dmv({ model: model, view: view, callback: callback });

        return $(dlgName).dialog("open");
    }



    self.OpenDialog = function (model, callback, view) {

        return self.OpenDialogWithName("#dialog-placeholder", model, callback, view);
    }



    ///
    self.CloseDialog = function () {
        if (self.dmv().callback != null && self.dmv().callback.save != null)
            return self.dmv().callback.save(self.dmv().model);
        return true;
    }

    ///
    self.CancelDialog = function () {
        return true;
    }


    self.dmvErrorInit = { view: null, model: null, callback: null };
    self.dmvError = ko.observable(self.dmvErrorInit);


    self.errorDialogInit = function (elements) {
        if (self.dmvError().model != null && self.dmvError().model.init !== undefined) {
            self.dmvError().model.init(elements);
        }
        else {
        }
    }

    self.OpenErrorDialog = function (model, callback, view) {

        self.dmvError({ model: model, view: view, callback: callback });

        return $("#error-dialog-placeholder").dialog("open");
    }

    self.CancelErrorDialog = function () {
        return true;
    }


    //
    //http://stackoverflow.com/questions/68485/how-to-show-loading-spinner-in-jquery
    //
    $(document)
     .ajaxStart(function () {
         if (erp.settings.isWaitCursorOn()) {
             $("#spinnerImage").show();
         }
     })
     .ajaxStop(function () {
         if (erp.settings.isWaitCursorOn()) {
             $("#spinnerImage").hide();
         }
     });



}
