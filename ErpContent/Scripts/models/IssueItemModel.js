function IssueItemModel(src, docRoot) {
    var self = this;

    var dt2js = function (options) { return erp.utils.Json2Date(options.data) };

    self.toObject = function () {
        return ko.mapping.toJS(self);
    }

    self.fromObject = function (o) {

        ko.mapping.fromJS(o  // source
            , {
                "publicationDate": { create: dt2js }
            , "createdDate": { create: dt2js }
            , "dateCreated": { create: dt2js }
            , "dateUpdated": { create: dt2js }
            }
            , self    // target
            );
    }

    self.fromObject(src);


    self.loaded = false;
    self.assignmentList = ko.observableArray();
    self.workflow = ko.observable(0);


    self.Publish = function () {

        $.ajax({
            type: 'POST',
            url: docRoot +  'Issue/IssueInfo',
            data: { id: self.id() },
            success: function (r) {
                var m = {};

                m.init = function (elements) {
                };

                m.save = function (o) {
                    if (o.notesApproved > 0) {
                        $.ajax({
                            async: false,   // block
                            type: 'POST',
                            url: docRoot + 'Issue/Publish',
                            data: { id: self.id() },
                            success: function (r) {
                                self.wfState(100);

                                ko.utils.arrayForEach(self.assignmentList(), function (item) {
                                    item.wfState(100);
                                });

                            },
                            error: function (request, status, error) {
                                PAGE_CONTEXT.error(request, status, error);
                            }
                        });
                    }
                    return true;
                };


                var d = pageData.OpenDialogWithName("#dialog-placeholder", r, m, "publish-dialog-template");
                d.dialog("option", "title", "Publish Issue");

            },
            error: function (request, status, error) {

                PAGE_CONTEXT.error(request, status, error)
            }
        });

    };


    self.PublishRollack = function () {


        $.ajax({
            type: 'POST',
            url: docRoot + 'Issue/IssueInfo',
            data: { id: self.id() },
            success: function (r) {


                var m = {};

                m.init = function (elements) {
                };

                m.save = function (o) {


                    if (o.notesPublished > 0) {
                        $.ajax({
                            async: false,   // block
                            type: 'POST',
                            url: docRoot + 'Issue/RollbackPublish',
                            data: { id: self.id() },
                            success: function (r) {
                                self.wfState(0);
                                ko.utils.arrayForEach(self.assignmentList(), function (item) {
                                    item.wfState(0);
                                });
                            },
                            error: function (request, status, error) {

                                PAGE_CONTEXT.error(request, status, error);
                            }
                        });
                    }
                    return true;
                };


                var d = pageData.OpenDialogWithName("#dialog-placeholder", r, m, "publish-dialog-template");
                d.dialog("option", "title", "Rollback Issue Publishing");

            },
            error: function (request, status, error) {

                PAGE_CONTEXT.error(request, status, error)
            }
        });


    };



    self.Export = function () {

        erp.utils.ajaxDownload(docRoot + 'Issue/Export"', { id: self.id() });

    };



    self.OpenDialogNewAssgnmt = function () {


        $.ajax({
            type: 'POST',
            url: docRoot + 'Assignment/GetNewAssignment',
            data: {},
            success: function (r) {
                //
                // use return value as template
                //
                //
                var m = new AssignmentModel(r);

                m.issue(self.title());
                m.issueId(self.id());
                m.publicationId(self.publicationID());
                m.publication(self.publicationName());


                m.init = function (elements) {
                    $(elements).find("#assgnmtSbmtDate").datepicker();
                    $(elements).find("#assgnmtPRDate").datepicker();
                    $(elements).find("#assgnmtAppDate").datepicker();

                    m.validator = $("#assignment-edit-template-form").validate();
                };

                m.validate = function () {
                    var result = true;
                    result = m.validator.form();
                    if (!result) {
                        m.validator.showErrors();
                    }
                    return result;
                }


                m.save = function (o) {

                    if (!m.validate()) {
                        return false;
                    }

                    $('.dropdown-error').html('');

                    if (o.author == null) {
                        $('#author-error-container').html("* Please select an author");
                        return false;
                    }

                    if (o.editor == null) {
                        $('#editor-error-container').html("* Please select an editor");
                        return false;
                    }

                    if (o.proofread == null) {
                        $('#proofread-error-container').html("* Please select a proof reader");
                        return false;
                    }

                    if (checkDeadlineValidities(o)) {
                        $.ajax({
                            type: 'POST',
                            url: '@Url.Content("~/Assignment/AddAssignment")',
                            data:
                                {
                                    str: JSON.stringify(m.toObject())
                                },
                            success: function (r) {
                                var v = new AssignmentModel(r);
                                self.assignmentList.unshift(v);
                            },
                            error: function (request, status, error) {

                                PAGE_CONTEXT.error(request, status, error)
                            }
                        });

                        return true;
                    }

                    return false;

                };



                var d = pageData.OpenDialog(m, m, "assignment-edit-template");
                d.dialog("option", "title", "Create New assignment");


            },
            error: function (request, status, error) {

                PAGE_CONTEXT.error(request, status, error)

            }
        });




    }

    self.deleteAssignment = function (objectToEvaluate) {

        var confirmation = confirm('Are you sure you want to delete this assignment?');

        if (confirmation == true) {

            $.ajax({
                type: 'POST',
                url: docRoot + 'Assignment/DeleteAssignment',
                data: { assignmentID: objectToEvaluate.id },
                success: function (r) {
                    $('#assignment_' + objectToEvaluate.id).remove();
                },
                error: function (request, status, error) {

                    PAGE_CONTEXT.error(request, status, error)

                }
            });

        }

        return false;
    }



    self.edit = function () {
        var m = new IssueModel(self.toObject());


        //
        // will be call after dialog initialization
        //
        m.init = function (elements) {

            $(elements).find("#issue-edit-dialog-create-date").datepicker();
            $(elements).find("#issue-edit-dialog-publish-date").datepicker();
            $(elements).find("#issue-edit-dialog-proof-date").datepicker();

            m.validator = $("#issue-edit-template-form").validate({ debug: true });

        };

        m.validate = function () {

            var result = true;
            result = m.validator.form();

            if (!result)
                m.validator.showErrors();

            return result;
        }


        m.save = function (o) {


            if (!m.validate()) {
                return false;
            }

            o.publicationID(o.selPublication().id);
            o.publicationName(o.selPublication().name)

            $.ajax({
                type: 'POST',
                url: docRoot + 'Issue/EditIssue',
                data: { str: JSON.stringify(o.toObject()) },
                success: function (r) {
                    self.fromObject(r);
                },
                error: function (request, status, error) {

                    PAGE_CONTEXT.error(request, status, error)

                }
            });
            return true;
        };


        m.selPublication = ko.observable(erp.utils.FindItemById(m.publicationID(), PAGE_CONTEXT.publications));

        var d = pageData.OpenDialog(m, m, "issue-edit-template");

        d.dialog("option", "title", "Edit Issue");

    }

    self.drillDown = function () {

        if (!self.loaded) {
            self.loaded = !self.loaded;
            //$.get(docRoot + 'Assignment/GetAssignmentsByIssue', { issueId: self.id() },
            $.get(docRoot + 'Assignment/GetAssignmentsByIssueByUser', { issueId: self.id() },
                 function (result) {

                     var t = ko.mapping.fromJS(
                             { 'children': result },
                             {
                                 'children':
                                    {
                                        create: function (options) {
                                            var result = new AssignmentModel(options.data,docRoot);
                                            return result;
                                        }
                                    }
                             }, {});


                     if (t.children().length == 0) {
                         alert("there are no assignments for that issue assigned to your account.");
                     }

                     self.assignmentList(t.children());
                 });

        }
        pageData.drillDownExt('issue-details-template', self, self.title());
    }
}
