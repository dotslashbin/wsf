﻿

function AssignmentModel(src, docRoot) {
    var self = this;


    var dt2js = function (options) { return erp.utils.Json2Date(options.data) };
    var dt2jsYear = function (options) { return erp.utils.Json2Year(options.data) };
    //
    // 
    //

    self.events = ko.observableArray();

    self.toObject = function () {
        var o = ko.mapping.toJS(self);
        return o;
    }

    self.fromObject = function (o) {
        ko.mapping.fromJS(o,
            {
                'copy': [
                    "author",
                    "editor",
                    "proofread"]
                , "submitDate":    { create: dt2js }
                , "proofreadDate": { create: dt2js }
                , "approveDate":   { create: dt2js }
                , "publishDate":   { create: dt2js }
            }, self);
    }

    //
    //
    //
    if (src)
        self.fromObject(src);



    self.goToDetails = function () {

        pageData.drillDownExt('assignment-details-template', self, self.title());
    }



    self.deleteTastingEvent = function (item) {

        if (!window.confirm("Do you really want delete '" + item.title() + "' tasting record?")) {
            return;
        }

        $.ajax({
            type: 'POST',
            url: docRoot + 'TastingEvent/DeleteTastingEvent',
            data:
                {
                    str: JSON.stringify(item.toObject())
                },
            success: function (r) {
                if (r)
                    self.events.remove(item);
            },
            error: function (request, status, error) {

                var dlg = pageData.OpenErrorDialog({ error: request.responseText }, null, "error-view-template");

                dlg.dialog("option", "title", "Server Error").
                    dialog("option", "width", 600).
                    dialog("option", "height", 300);

            }
        });

    };


    self.editTastingEvent = function (item) {
        try {
            var m = new TastingEventModel(item.toObject(), docRoot );

            m.init = function () {
                initTastingEventEditForm(m);
            };

            m.save = function (o) {

                $.ajax({
                    type: 'POST',
                    url: docRoot + 'TastingEvent/EditTastingEvent',
                    data:
                        {
                            str: JSON.stringify(m.toObject())
                        },
                    success: function (r) {

                        item.fromObject(r);
                    },
                    error: function (request, status, error) {

                        var dlg = pageData.OpenErrorDialog({ error: request.responseText }, null, "error-view-template");

                        dlg.dialog("option", "title", "Server Error").
                            dialog("option", "width", 600).
                            dialog("option", "height", 300);

                    }
                });

                return true;
            };

            m.assignmentId = self.id();

            var dlg = pageData.OpenDialog(m, m, "tasting-event-template");

            dlg.dialog("option", "title", "Edit Tasting Record").
                dialog("option", "width", 600).
                dialog("option", "height", 350);

        } catch (e) {
            alert(e);
        }

    }





    self.createNote = function (item) {

        var m = new TastingNoteModel(item, '@Url.Content("~")');


        //m.tastingEventId = item.id;
        m.init = function (elements) {
            initNoteEditForm(elements, m, '@Url.Content("~")');
        };

        m.save = function (o) {

            $.ajax({
                type: 'POST',
                url: docRoot + 'TastingNote/AddTastingNote',
                data:
                    {
                        str: JSON.stringify(o.toObject())
                    },
                success: function (r) {

                    r.editNote = self.editNote;
                    r.approveNote = self.approveNote;
                    r.sendBackNote = self.sendBackNote;
                    item.notes.unshift(r);

                },
                error: function (request, status, error) {


                    var dlg = pageData.OpenErrorDialog({ error: request.responseText }, null, "error-view-template");

                    dlg.dialog("option", "title", "Server Error").
                        dialog("option", "width", 600).
                        dialog("option", "height", 300);

                }
            });

            return true;
        };


        var dlg = pageData.OpenDialog(m, m, "tasting-note-template");

        dlg.dialog("option", "title", "Create New Note").
            dialog("option", "width", 850).
            dialog("option", "height", 600);

    }



    self.setApproved = function () {


        var result = window.confirm("You are about to set all notes in this\r\nassignment as 'approved'.\r\n\r\nDo you wish to continue?");
        if (!result)
            return;




        $.ajax({
            type: 'POST',
            url: docRoot + 'Assignment/SetAssignmentApproved',
            data:
                {
                    assignmentId: self.id
                },
            success: function (r) {
                //
                // reload
                //
                self.load();
            }
        });

    }


    /*
    self.createNoteFromExisting = function (tastingEvent) {

        //
        //
        //
        VinSearchModel.tastingEvent = tastingEvent;

        //
        //
        //
        pageData.drillDownExt("search-tasting-note-template-window", VinSearchModel, "Search Existing Notes");
    }
    */

    //
    // it is confusing how we should call this method. I do not undestand myself how this binding works :-(
    // click : $parents[1].assignment.moveTastingNote.bind($data,$parent)
    //
    self.moveTastingNote = function (tastingRecord, note) {


        try {

            $.get(docRoot + 'Assignment/GetAssignmentsByUser', { publicationid: 0, state: '1' },
                    function (result) {

                        /**/
                        var t = ko.mapping.fromJS(
                            { 'children': result },
                            {
                                'children': {
                                    create: function (options) {
                                        var result = new AssigmentModel(options.data);
                                        return result;
                                    }
                                }
                            }, {});

                        var m = {};

                        m.init = function () {
                        };




                        m.save = function (o) {


                            if (!m.moveToAssignmentId()) { // no selection
                                alert("Please select assginment where you want move this tastging record");
                                return false;
                            }

                            if (!m.moveToEventId()) { // no selection
                                alert("Please select Tasting Record where you want move this tastging record");
                                return false;
                            }

                            if (m.moveToAssignmentId() == self.id() && m.moveToEventId() == tastingRecord.id) { // select move to itself
                                alert("Tastging record already in the assignment you selected ");
                                return false;
                            }


                            $.ajax({
                                type: 'POST',
                                url: docRoot +  'TastingNote/MoveTastingNote',
                                data:
                                    {
                                        tastingEventId: m.moveToEventId(),
                                        tastingNoteId: note.id
                                    },
                                success: function (r) {
                                    tastingRecord.notes.remove(note);
                                    if (m.moveToAssignmentId() == self.id()) {
                                        for (var i = 0; i < self.events().length; i++) {
                                            if (self.events()[i].id == m.moveToEventId()) {
                                                self.events()[i].notes.unshift(note);
                                                break;
                                            }
                                        }
                                    }

                                },
                                error: function (request, status, error) {
                                    var dlg = pageData.OpenErrorDialog({ error: request.responseText }, null, "error-view-template");
                                    dlg.dialog("option", "title", "Server Error").
                                        dialog("option", "width", 600).
                                        dialog("option", "height", 300);
                                }
                            });

                            return true;
                        };

                        m.assignments = t.children;
                        m.events = ko.observableArray();
                        m.moveToAssignmentId = ko.observable(null);
                        m.moveToEventId = ko.observable(null);


                        m.moveToAssignmentId.subscribe(function (newValue) {

                            if (newValue) {
                                $.get(docRoot + 'TastingEvent/GetTastingEventByAssignment', { assignmentId: newValue },
                                function (result) {
                                    var t = ko.mapping.fromJS(
                                        { 'children': result },
                                        {
                                            'children':
                                            {
                                                create: function (options) {
                                                    var r = new TastingEventModel(options.data, docRoot);
                                                    return r;
                                                }
                                            }
                                        }, {});

                                    m.events(t.children());
                                });
                            }
                        });

                        var dlg = pageData.OpenDialog(m, m, "tasting-note-move-template");
                        dlg.dialog("option", "title", "Move Tasting Note").
                            dialog("option", "width", 600).
                            dialog("option", "height", 350);
                    });


        } catch (e) {
            alert(e);
        }


    }



    self.moveTastingEvent = function (item) {

        try {

            $.get(docRoot +  'Assignment/GetAssignmentsByUser', { publicationid: 0, state: '1' },
                    function (result) {

                        /**/
                        var t = ko.mapping.fromJS(
                            { 'children': result },
                            {
                                'children': {
                                    create: function (options) {
                                        var result = new AssigmentModel(options.data);
                                        return result;
                                    }
                                }
                            }, {});

                        var m = {};

                        m.init = function () {
                        };

                        m.save = function (o) {


                            if (!m.moveToAssignmentId()) { // no selection
                                alert("Please select assginment where you want move this tastging record");
                                return false;
                            }

                            if (m.moveToAssignmentId() == self.id()) { // select move to itself
                                alert("Tastging record already in the assignment you selected ");
                                return false;
                            }


                            $.ajax({
                                type: 'POST',
                                url: docRoot +  'TastingEvent/MoveTastingEvent',
                                data:
                                    {
                                        assignmentId: m.moveToAssignmentId(),
                                        tastingEventId: item.id
                                    },
                                success: function (r) {
                                    self.events.remove(item);
                                },
                                error: function (request, status, error) {
                                    var dlg = pageData.OpenErrorDialog({ error: request.responseText }, null, "error-view-template");
                                    dlg.dialog("option", "title", "Server Error").
                                        dialog("option", "width", 600).
                                        dialog("option", "height", 300);
                                }
                            });

                            return true;
                        };

                        m.assignments = t.children;
                        m.moveToAssignmentId = ko.observable(null);

                        var dlg = pageData.OpenDialog(m, m, "tasting-event-move-template");
                        dlg.dialog("option", "title", "Move Tasting Record").
                            dialog("option", "width", 600).
                            dialog("option", "height", 350);
                    });


        } catch (e) {
            alert(e);
        }
    }


    self.TastingEventFactory = function (src, urlRoot) {
        var r = new TastingEventModel(src, urlRoot);
        //r.open = ko.observable(false);
        //r.loaded = ko.observable(false);
        //r.sortById = ko.observable(0);
        //r.sortBy = self.sortBy;
        //r.showNotes = eventShowItemsCallback;
        return r;
    }

    self.load = function () {
        $.get(docRoot + 'TastingEvent/GetTastingEventByAssignment', { assignmentId: self.id },
             function (result) {

                 var t = ko.mapping.fromJS(
                       { 'children': result },
                       { 'children':  {
                                  create: function (options) {
                                      return self.TastingEventFactory(options.data, docRoot);
                                  }
                              }
                       }, {});

                 self.events(t.children());
             });
    }


    self.goToAssignmentDetails = function (issue) {

        if (!self.loaded) {
            self.loaded = !self.loaded;
            self.load();
        }

        pageData.drillDownExt('assignment-details-template', new NotesModel(issue, self), self.title());
    }


    self.edit = function () {
    }



}


