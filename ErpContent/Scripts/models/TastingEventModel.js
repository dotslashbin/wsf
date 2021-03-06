﻿

    function TastingEventModel(src) {
        var self = this;

        var dt2js = function (options) { return erp.utils.Json2Date(options.data) };
        //
        // 
        //

        //self.title = ko.observable('');
        //self.location = ko.observable('');
        //self.comments = ko.observable('');

        self.notes = ko.observableArray();

        self.open = ko.observable(false);
        self.loaded = ko.observable(false);
        self.sortById = ko.observable(0);
        self.sortBy = self.sortBy;




        self.toObject = function () {
            return ko.mapping.toJS(self);
        }

        self.refreshCounts = function () {

            if (self.notes().length == 0)
                return;

            self.notesCount(self.notes().length);
            var draftCount = 0;
            var proofreadCount = 0;
            var editorCount = 0;


            for (var i = 0; i < self.notes().length; i++) {

                if( self.notes()[i].wfState() == 0 )
                    draftCount++;

                if (self.notes()[i].wfState() == 10)
                    proofreadCount++;

                if (self.notes()[i].wfState() == 50)
                    editorCount++;
            }

            self.draftCount(draftCount);
            self.proofreadCount(proofreadCount);
            self.editorCount(editorCount);

        }


        self.showNotes = function (item) {
            if (!item.loaded()) {
                $.get(erp.wsf_path +  'TastingNote/GetNotesByTastingEvent', { eventId: item.id },
                function (result) {
                    var t = ko.mapping.fromJS(
                        { 'children': result },
                        { 'children':
                               {
                                   create: function (options) {
                                       var tnm = new TastingNoteModel(options.data);
                                       return tnm;
                                   }
                               }
                        }, {});

                    item.loaded(true);
                    item.notes(t.children());
                    item.open(!item.open());
                    item.refreshCounts();
                });
            } else {
                item.open(!item.open());
            }
        }




        self.showUploadForm = function (tastingEvent) {
            var dlg = pageData.OpenErrorDialog(this, this, "tasting-note-upload-form");

            dlg.dialog("option", "title", "Upload tasting notes").
            dialog("option", "width", 600).
            dialog("option", "height", 300);

            $('#import-tasting-note-form').submit(
                function (event) {

                    event.preventDefault();

                    // Validation
                    if ($('#import-notes-field-container').val() == null || $('#import-notes-field-container').val() == '') {
                        alert("Please choose a the file to upload.");

                        return false;
                    }

                    var formData = new FormData($(this)[0]);

                    $.ajax({
                        url: $('#import-url-container').val(),
                        type: 'POST',
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (tastingNotes) {

                            for (var iterator = 0; iterator < tastingNotes.length; iterator++) {
                                var tastingNote = tastingNotes[iterator];

                                var result = new TastingNoteModel(tastingNote);
                                //result.editNote = editNoteCallback;
                                //result.setReadyNote = setReadyNoteCallback;
                                //result.setDraftNote = setDraftNoteCallback;

                                tastingEvent.notes.unshift(result);
                            }
                        }
                    });

                    $('div.ui-dialog-buttonpane div button').trigger('click');

                    return false; 
                }
            );
        }

        self.fromObject = function (o) {
            ko.mapping.fromJS(o,
                {
                    'copy': ["id", "assignmentId"]
                    , 'created': { create: dt2js }
                },
                self);
        }

        self.fromObject(src);


        self.createBareNote = function (tastingEvent,vin,vintage, callback) {

            var postParams = {};

            postParams.eventId = tastingEvent.id;

            if( vin )
                postParams.vin = JSON.stringify(vin.toObject());

            if (vintage)
                postParams.vintage = vintage;


            $.ajax({
                type: 'POST',
                url: erp.wsf_path + 'TastingNote/GetNewNoteForEvent',
                data: postParams,
                success: function (r) {
                        callback(r);
                },

                error: function (request, status, error) {
                    alert("error while creating note's placeholder. report to admin");

                }
            });

        }

        self.createNote = function (tastingEvent,event, vin, vintage) {

            self.createBareNote(tastingEvent,vin,vintage, function (r) {
                //
                // use return value as template
                //
                //
                var m = new TastingNoteModel(r);


                if (vin) {
                    m.vinEditable(false);
                    m.vintageEditable(true);

                    $.get(erp.wsf_path + 'TastingNote/GetNotesByVinN', { vinN: vin.id() },
                                 function (result) {

                                     var t = ko.mapping.fromJS(
                                         { 'children': result },
                                         {
                                             'children':
                                                {
                                                    create: function (options) {
                                                        var result = new TastingNoteModel(options.data);
                                                        return result;
                                                    }
                                                }
                                         }, {});

                                     m.history(t.children());
                                 });
                }

                if (vintage) {
                    m.vintageEditable(false);
                }

                m.init = function (elements) {
                    initNoteEditForm(elements, m);



                    m.validator = $("#tasting-note-form-validation").validate(
                        {
                            debug: true,
                            rules:
                                {

                                    'note-edit-vintage': {
                                        required: true,
                                        vintage: true
                                    },

                                    'note-edit-rating': {
                                        required: false,
                                        rating: true
                                    },

                                    'note-edit-drink-from': {
                                        required: false,
                                        range: [2000, 2400]
                                    },
                                    'note-edit-drink-to': {
                                        required: false,
                                        range: [2000, 2400]
                                    },
                                    'note-edit-estimated-cost-low': {
                                        required: false,
                                        number: true
                                    },
                                    'note-edit-estimated-cost-low': {
                                        required: false,
                                        number: true
                                    }

                                }
                        }
                        );
                };


                m.validate = function () {
                    var validationResult = true;
                    validationResult = m.validator.form();
                    if (!validationResult)
                        m.validator.showErrors();
                    return validationResult;
                }

                m.areDateRangesValid = function (objectToSave) {

                    var dateFrom = parseInt(objectToSave.drinkDateLo);
                    var dateTo = parseInt(objectToSave.drinkDateHi);

                    if (dateTo >= dateFrom) {
                        return true;
                    }

                    return false;
                }


                m.save = function (o) {
                    //
                    // this is ugly. at the moment do not know how make it better.
                    // activate main tab first, otherwise validation does not work properly
                    //
                    $('#tasting-note-form-validation #note-props-tab').tab('show');



                    if (!m.validate())
                        return;


                    /**
                    * This function call checks whether the "Date To" field is greater than the "Date From". An 
                    * error message will be displayed if this fails validation
                    */
                    /*
                    if (m.areDateRangesValid(o) == false) {
                        $('#date-to-field-container').append('<label class="error" for="note-edit-drink-to">This is not a valid date range</label>');
                        return false;
                    }
                    */

                    $.ajax({
                        type: 'POST',
                        url: erp.wsf_path + 'TastingNote/AddTastingNote',
                        data:
                            {
                                str: JSON.stringify(o.toObject())
                            },
                        success: function (r) {


                            var result = new TastingNoteModel(r);
                            //result.editNote = editNoteCallback;
                            //result.setReadyNote = setReadyNoteCallback;
                            //result.setDraftNote = setDraftNoteCallback;


                            tastingEvent.notes.unshift(result);
                            pageData.drillUp();

                        },
                        error: function (request, status, error) {

                            alert('Error while saving a tasting note. Please report to admin.');

                        }
                    });

                };

                //
                pageData.drillDownExt("tasting-note-template-window", m, "Create/Edit Note");

            });


        }






        self.deleteTastingNote = function (item) {

            if (!window.confirm("Do you really want delete tasting note?")) {
                return;
            }

            $.ajax({
                type: 'POST',
                url: erp.wsf_path + 'TastingNote/DeleteTastingNote',
                            data:
                                {
                                    str: JSON.stringify( item.toObject())
                                },
                            success: function (r) {
                                if( r )
                                    self.notes.remove(item);
                            },
                            error: function (request, status, error) {

                                var dlg = pageData.OpenErrorDialog({ error: request.responseText }, null, "error-view-template");

                                dlg.dialog("option", "title", "Server Error").
                                    dialog("option", "width", 600).
                                    dialog("option", "height", 300);

                            }
                        });

        };


    };

    function initTastingEventEditForm(model) {

        var self = this;
        self.model = model;

    }


