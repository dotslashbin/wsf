function IssueListModel( ) {


    var self = this;

    self.issueList = ko.observableArray();

    self.publications = ko.observableArray();
    self.publicationId = ko.observable(0);

    self.stateGroupId = ko.observable(0);
    self.stateGroups = ko.observableArray();


    self.initOnce = false;

    self.init = function () {

        //
        // if you change IDs, please be sure they are in synch with values defined in EditorsCommon.WorkFlowState 
        //
        //public const int STATE_GROUP_IN_PROCESS = 1;
        //public const int STATE_GROUP_PUBLISHED = 2;
        //public const int STATE_GROUP_ARCHIVED = 3;
        //public const int STATE_GROUP_ALL = 99;

        if( ! self.initOnce ){

            self.stateGroups([
                {name:'ALL'       ,id:'99'},
                {name:'In Process',id:'1'},
                {name:'Published' ,id:'2'},
            ]);

            self.stateGroupId('1');


            self.loadPublications();
            self.loadIssues();
            self.initOnce = !self.initOnce;
        }

    }

    self.loadPublications = function () {

        $.get(erp.wsf_path +  'ChiefEditor/Publications', {},
                    function (result) {
                        self.publications(result);
                        PAGE_CONTEXT.publications = result;
                    });
    }



    self.loadIssues = function () {

        var publicationId = self.publicationId() || 0;
        var stateGroup = self.stateGroupId();

        if (stateGroup != '1' && publicationId == 0) {
            alert("Your choice is too broad. Please select a publication other than ALL");
            return;
        }

        self.load(publicationId,stateGroup );
    }

    self.addPublication = function () {
        $.ajax({
            type: 'POST',
            url:  erp.wsf_path +  'Publication/GetPublications',
            data: {},
            success: function (r) {


                var m = new PublicationModel(r);

                m.init = function () {
                    //Romel -edited June 18,2014
                    //fixed the date picker issue
                    $("#add-publication-dialog-create-date").datepicker();
                    m.validator =   $("#add-publication-template-form").validate({debug : true});
                };


                m.validate = function () {
                    var validationresult = true;
                    validationresult =  m.validator.form();
                    if( ! validationresult )
                        m.validator.showerrors();
                    return validationresult;
                }

                //Save

                m.save = function (o) {

                    //Romel -edited June 18,2014
                    //fixed the date picker issue
                    if (!m.validate()) {
                        return false;
                    }

                    o.PublisherId('1');
                    o.ID('0');
                    $.ajax({
                        type: 'POST',
                        url: erp.wsf_path +  'Publication/AddPublication',
                        data: {PublicationData: JSON.stringify(o.toObject())},
                        success: function (result) {
                            if (result != null) {
                                if (!result.Errorexist) {
                                    $.get(erp.wsf_path + 'ChiefEditor/Publications', {},
                                       function (result) {
                                           self.publications(result);
                                           PAGE_CONTEXT.publications = result;
                                       });
                                } else {
                                    //show error message
                                    if (result.ErrorMessage != "") {
                                        alert(result.ErrorMessage);
                                        page_context.error(request, status, error)
                                        return false;
                                    }
                                }
                            }
                        },
                        error: function (request, status, error) {
                            alert(error)
                            page_context.error(request, status, error)
                        }
                    });
                    return true;
                }


                var d = pageData.OpenDialog(m, m, "add-publication-template");

                d.dialog("option", "title", "Create New Publication").
                dialog("option", "width", 760).
                dialog("option", "height", 400);

            },
            error: function (request, status, error) {

                PAGE_CONTEXT.error(request, status, error)

            }
        });
    };

    self.addIssue = function () {
        $.ajax({
            type: 'POST',
            url: erp.wsf_path +  'Issue/GetNewIssue',
            data: {},
            success: function (r) {
                //
                // use return value as template
                //
                //
                var m = new IssueModel(r);

                m.initCount = 0;
                //
                // will be called after dialog initialization
                //
                m.init = function (elements) {

                    $(elements).find("#issue-edit-dialog-create-date").datepicker();
                    $(elements).find("#issue-edit-dialog-publish-date").datepicker();
                    $(elements).find("#issue-edit-dialog-proof-date").datepicker();

                    m.validator =   $("#issue-edit-template-form").validate({debug : true});
                };


                m.validate = function(){
                    var validationResult = true;
                    validationResult =  m.validator.form();
                    if( ! validationResult )
                        m.validator.showErrors();
                    return validationResult;

                    return true;
                }

                //
                // will be called after "save changes" clicked
                //
                m.save = function (o) {

                    $('#add-issue-published-field').html(''); 
                    $('#issue-edit-dialog-publish-date').removeClass('error'); 

                    if( !m.validate()){
                        return false;
                    }

                    if (Date.parse(o.createdDate) > Date.parse(o.publicationDate)) {
                        $('#issue-edit-dialog-publish-date').addClass('error');
                        $('#add-issue-published-field').html("Invalid Date Range!\nCreate Date cannot be after Publish Date!"); 
                        return false;
                    }

                    o.publicationID(o.selPublication().id);
                    o.publicationName(o.selPublication().name)
                            
                    $.ajax({
                        type: 'POST',
                        url: erp.wsf_path +  'Issue/AddIssue',
                        data: { str: JSON.stringify(o.toObject()) },
                        success: function (r) {
                            if (r != null) {

                                var v = new IssueModel(r);
                                self.issueList.unshift(v);
                            }
                        },
                        error: function (request, status, error) {

                            PAGE_CONTEXT.error(request, status, error)

                        }
                    });

                    return true;

                }

                m.selPublication = ko.observable(null);

                var d = pageData.OpenDialog(m, m, "issue-edit-template");

                d.dialog("option", "title", "Create New Issue").
                dialog("option", "width", 760).
                dialog("option", "height", 400);


            },
            error: function (request, status, error) {

                PAGE_CONTEXT.error(request, status, error)

            }
        });
    };


    self.deleteIssue = function(item)
    {
        if( window.confirm("Do you want delete issue '" + item.title() + "'?" ) ) {
            $.ajax({
                type: 'POST',
                url: erp.wsf_path +  'Issue/DeleteIssue',
                data:
                    {
                        str: JSON.stringify(item.toObject())
                    },
                success: function (r)
                {
                    self.issueList.remove(item);
                },
                error: function (request, status, error) {
                    PAGE_CONTEXT.error(request, status, error)
                }
            });
        }
    }


    self.load = function (publicationId, state) {

        $.get(erp.wsf_path +  'Issue/Issues', { publicationId: publicationId, state : state },
                 function (result) {

                     var t = ko.mapping.fromJS(
                         {'children' : result},
                         {'children':
                             {
                                 create: function(options) {
                                     return new IssueModel(options.data);
                                 }
                             }},{});


                     self.issueList(t.children());

                 });
    }

}
