

    function TastingEventModel(src, docRoot) {
        var self = this;

        var dt2js = function (options) { return erp.utils.Json2Date(options.data) };
        //
        // 
        //
        self.docRoot = docRoot;

        self.title = ko.observable('');
        self.location = ko.observable('');
        self.comments = ko.observable('');
        self.notes = ko.observableArray();

        self.toObject = function () {
            return ko.mapping.toJS(self);
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
                                result.editNote = editNoteCallback;
                                result.setReadyNote = setReadyNoteCallback;

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

        self.createNote = function (tastingEvent,event, vin, vintage) {


            $.ajax({
                type: 'POST',
                url: docRoot +  'TastingNote/GetNewNoteForEvent',
                data:
                    {
                        eventId: tastingEvent.id
                    },
                success: function (r) {
                    //
                    // use return value as template
                    //
                    //
                    var m = new TastingNoteModel(r);


                    if ( vin ) {
                        m.loadFromVin(vin);
                        m.vinEditable(false);
                        m.vintageEditable(true);

                        $.get(docRoot +  'TastingNote/GetNotesByVinN', { vinN: vin.id() },
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

                    if ( vintage) {
                        m.vintage(vintage.vintage());
                        m.vintageEditable(false);
                    }

                    m.init = function (elements) {
                        initNoteEditForm(elements, m, self.docRoot);



                        m.validator = $("#tasting-note-form-validation").validate(
                            {
                                debug: true,
                                rules:
                                    {

                                        'note-edit-rating': {
                                            required: false,
                                            range: [60, 100]
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
                            url: docRoot + 'TastingNote/AddTastingNote',
                            data:
                                {
                                    str: JSON.stringify(o.toObject())
                                },
                            success: function (r) {


                                var result = new TastingNoteModel(r);
                                result.editNote = editNoteCallback;
                                result.setReadyNote = setReadyNoteCallback;

                                tastingEvent.notes.unshift(result);
                                pageData.drillUp();

                            },
                            error: function (request, status, error) {

                                var dlg = pageData.OpenErrorDialog({ error: request.responseText }, null, "error-view-template");

                                dlg.dialog("option", "title", "Server Error").
                                    dialog("option", "width", 600).
                                    dialog("option", "height", 300);

                            }
                        });

                    };

                    //
                    pageData.drillDownExt("tasting-note-template-window", m,"Create/Edit Note");



                },
                error: function (request, status, error) {


                    var dlg = pageData.OpenErrorDialog({ error: request.responseText }, null, "error-view-template");

                    dlg.dialog("option", "title", "Server Error").
                        dialog("option", "width", 600).
                        dialog("option", "height", 300);

                }
            });


        }






        self.deleteTastingNote = function (item) {

            if (!window.confirm("Do you really want delete tasting note?")) {
                return;
            }

            $.ajax({
                type: 'POST',
                url: docRoot + 'TastingNote/DeleteTastingNote',
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


