function IssueListUserModel( ) {


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



    self.load = function (publicationId, state) {

        $.get(erp.wsf_path +  'Issue/IssuesForUser', { publicationId: publicationId, state : state },
                 function (result) {

                     var t = ko.mapping.fromJS(
                         {'children' : result},
                         {'children':
                             {
                                 create: function(options) {
                                     return new IssueItemModel(options.data);
                                 }
                             }},{});


                     self.issueList(t.children());

                 });
    }

}
