

    // 03.29.14 require knockout.mapping
    //
    //
    //

    function TastingNoteModel(src, appRoot) {
        var self = this;

        self.appRoot = appRoot;
        //
        // dummy observale. will change value for each instance. useful to trac changes for computes which use nonobservales
        //
        self.ts = ko.observable(null);

        var dt2js     = function (options) { return erp.utils.Json2Date(options.data) };
        var dt2jsYear = function (options) { return erp.utils.Json2Year(options.data) };
        //
        // 
        //

        self.maturityLookup = [
            { id: 5, name: '' },
            { id: 0, name: 'Young' },
            { id: 1, name: 'Early' },
            { id: 2, name: 'Mature'},
            { id: 3, name: 'Late'}, 
            { id: 4, name: 'Old'}
        ];


        self.wineColorLookup = [
            { id: 0, name: '' },
            { id: 1, name: 'Red' },
            { id: 2, name: 'Rose' },
            { id: 3, name: 'White' },
        ];


        self.wineDrynessLookup = [
            { id: 0, name: '' },
            { id: 1, name: 'Dry' },
            { id: 2, name: 'Medium Dry' },
            { id: 3, name: 'Sweet' },
            { id: 4, name: 'Very Dry' },
        ];



        self.wineTypeLookup = [
            { id: 0	, name: '' },
            { id: 9, name: 'Table' },
            { id: 1	, name: 'Dessert' },
            { id: 2	, name: 'Fortified' },
            { id: 3	, name: 'Madeira' },
            { id: 4	, name: 'Port' },
            { id: 5	, name: 'Sake' },
            { id: 6	, name: 'Sherry' },
            { id: 7	, name: 'Sparkling' },
            { id: 8	, name: 'Sweet' }
        ];

        self.ratingQLookup = [
            { id: 0, name: '' },
            { id: 1, name: '*' },
            { id: 2, name: '+' },
            { id: 3, name: '+*'},
            { id: 4, name: '+?'}, 
            { id: 5, name: '?'}
        ];



        self.toObject = function () {

            var o = ko.mapping.toJS(self);

            o.drinkDateLo = erp.utils.Year2ValidDate(o.drinkDateLo);
            o.drinkDateHi = erp.utils.Year2ValidDate(o.drinkDateHi);
            //
            // to prevent error message about potential cross-siting scripting
            //
            o.noteFormated = "";

            return o;
        }

        self.fromObject = function (o) {

            ko.mapping.fromJS(o,
                {
                    'copy': ["id", "tastingEventId", "issueId"]
                    , "tastingDate": { create: dt2js }
                    , "drinkDateLo": { create: dt2jsYear }
                    , "drinkDateHi": { create: dt2jsYear }

                }, self);

            self.ts(new Date());


        }

        self.validateResult = "";
        self.validate = function () {


            self.validateResult = "";

            if (!self.note() || self.note().length == 0)
                self.validateResult += "\n\r" + "Tasting Note is empty";

            if (!self.rating() || self.rating().length == 0) {
                if (!self.ratingQ() || self.ratingQ().length == 0)
                  self.validateResult += "\n\r" + "Rating is invalid";
            }

            if (!self.drinkDateLo || self.drinkDateLo.length == 0) {
                if (!self.isBarrelTasting() || self.isBarrelTasting() == false)
                    self.validateResult += "\n\r" + "Drink From is invalid";
            }


            if (!self.drinkDateHi || self.drinkDateHi.length == 0) {
                if (!self.isBarrelTasting() || self.isBarrelTasting() == false)
                    self.validateResult += "\n\r" + "Drink To is invalid";
            }



            return self.validateResult.length == 0;
        }


        //
        //
        //
        if( src )
          self.fromObject(src);




        self.vinEditable = ko.observable(true);
        self.vintageEditable = ko.observable(false);

        self.history = ko.observableArray();

        self.loadFromVin = function (vin) {

            self.producer(vin.producer());
            self.wineName(vin.label());

            self.country(vin.country());
            self.region(vin.region());
            self.location(vin.location());
            self.locale(vin.locale());
            self.site(vin.site());

            self.variety(vin.variety());
            self.color(vin.colorClass());
            self.dryness(vin.dryness());

            self.wineType(vin.wineType());


            
        }




        self.coLocation = ko.computed(function () {

            var r = self.country;
            if (self.region && self.region() && self.region().length > 0)
                r += ", " + self.region();
            if (self.location && self.location() && self.location().length > 0)
                r += ", " + self.location();
            if (self.locale && self.locale() && self.locale().length > 0)
                r += ", " + self.locale();
            if (self.site && self.site() && self.site().length > 0)
                r += ", " + self.site();

            return r;
        }, self);

        self.coAppellation = ko.computed(function () {
            if (self.site && self.site() && self.site().length > 0) return self.site();
            if (self.locale && self.locale() && self.locale().length > 0) return self.locale();
            if (self.location && self.location() && self.location().length > 0) return self.location();
            if (self.region && self.region() && self.region().length > 0) return self.region();
            if (self.country && self.country() && self.country().length > 0) return self.country();
            return '';
        }, self);



        self.coDrink = ko.computed(function () {

            //
            // drinkDateLo and drinkDateHi are not observable,
            // we use ts here just to make dependency and force this computed 
            // invalidate drink value once instance is updated
            //
            if (self.ts && self.ts() ) {
                var dummy = self.ts();
            }

            if (self.drinkDateLo && self.drinkDateLo.length > 0) {
                var result = 'Drink ' + self.drinkDateLo;
                if (self.drinkDateHi  && self.drinkDateHi.length > 0) {
                    var result = result + '-' + self.drinkDateHi;
                }
                return result;
            }
            return '';
        }, self);

        self.coPrice = ko.computed(function () {

            //
            //
            //

            if (self.estimatedCost && self.estimatedCost() && self.estimatedCost().length > 0) {
                var result = 'Cost $' + self.estimatedCost();
                if (self.estimatedCostHi && self.estimatedCostHi() && self.estimatedCostHi().length > 0) {
                    var result = result + '-$' + self.estimatedCostHi();
                }
                return result;
            }

            return '';
        }, self);

    }


    function initNoteEditForm(elements, model, appRoot) {


        var self = this;
        self.model = model;
        self.appRoot = appRoot;

        self.null2str = function (s) {

            if (s )
                return s;

            return '';
        }


        $(elements).find("#note-edit-location").autocomplete(
        {
            source: self.appRoot + 'Reviewer/Appellation',
            focus: function (event, ui) {
                $("#note-edit-location").val(ui.item.appellation);
                return false;
            },
            select: function (event, ui) {
                $("#note-edit-location").val(ui.item.appellation);

                self.model.country(ui.item.country);
                self.model.region(ui.item.region);
                self.model.location(ui.item.location);
                self.model.locale(ui.item.locale);
                self.model.site(ui.item.site);
                return false;
            },
            minLength: 1
        }).data("ui-autocomplete")._renderItem = function (ul, i) {
            var c = null;
            if (i.site != null && i.site.length > 0) c = '';
            if (i.locale != null && i.locale.length > 0) c = c == null ? '' : (i.locale + (c.length > 0 ? (', ' + c) : ''));
            if (i.location != null && i.location.length > 0) c = c == null ? '' : (i.location + (c.length > 0 ? (', ' + c) : ''));
            if (i.region != null && i.region.length > 0) c = c == null ? '' : (i.region + (c.length > 0 ? (', ' + c) : ''));
            if (i.country != null && i.country.length > 0) c = c == null ? '' : (i.country + (c.length > 0 ? (', ' + c) : ''));
            return $("<li>").append("<a>" + i.appellation + " (" + c + ")</a>").appendTo(ul);
        };






        $(elements).find("#note-edit-location-country").autocomplete(
       {
           source: function (request, response) {
               $.get(self.appRoot + 'vin/SearchLocation',
                      {
                          c: request.term,
                          r: self.null2str(self.model.region()),
                          l: self.null2str(self.model.location()),
                          lc: self.null2str(self.model.locale()),
                          s: self.null2str(self.model.site())
                      },
                        function (result) { response(result); })
           },
           focus: function (event, ui) {
               $(this).val(ui.item.country);
               return false;
           },
           select: function (event, ui) {
               $(this).val(ui.item.country);
               return false;
           }, minLength: 1
       }).data("ui-autocomplete")._renderItem = function (ul, i) {
           return $("<li>").append("<a>" + i.country + "</a>").appendTo(ul);
       };





         $(elements).find("#note-edit-location-region").autocomplete(
         {
              source: function (request, response) {
                  $.get(self.appRoot + 'vin/SearchLocation',
                      {
                          c: self.null2str(self.model.country()),
                          r: request.term,
                          l: self.null2str(self.model.location()),
                          lc: self.null2str(self.model.locale()),
                          s: self.null2str(self.model.site())
                      },
                        function (result) { response(result); })
              },
              focus: function (event, ui) {
                  $(this).val(ui.item.region);
                  return false;
              },
              select: function (event, ui) {
                  $(this).val(ui.item.region);
                  return false;
              }, minLength: 1
          }).data("ui-autocomplete")._renderItem = function (ul, i) {
              return $("<li>").append("<a>" + i.region + "</a>").appendTo(ul);
          };



          $(elements).find("#note-edit-location-location").autocomplete(
          {
                 source: function (request, response) {
                     $.get(self.appRoot + 'vin/SearchLocation',
                      {
                          c: self.null2str(self.model.country()),
                          r: self.null2str(self.model.region()),
                          l: request.term,
                          lc: self.null2str(self.model.locale()),
                          s: self.null2str(self.model.site())
                      },
                        function (result) { response(result); })
              },
              focus: function (event, ui) {
                  $(this).val(ui.item.location);
                  return false;
              },
              select: function (event, ui) {
                  $(this).val(ui.item.location);
                  return false;
              }, minLength: 1
          }).data("ui-autocomplete")._renderItem = function (ul, i) {
              return $("<li>").append("<a>" + i.location + "</a>").appendTo(ul);
          };

          $(elements).find("#note-edit-location-locale").autocomplete(
          {
                 source: function (request, response) {
                     $.get(self.appRoot + 'vin/SearchLocation',
                      {
                          c: self.null2str(self.model.country()),
                          r: self.null2str(self.model.region()),
                          l: self.null2str(self.model.location()),
                          lc: request.term,
                          s: self.null2str(self.model.site())
                      },
                        function (result) { response(result); })
              },
              focus: function (event, ui) {
                  $(this).val(ui.item.locale);
                  return false;
              },
              select: function (event, ui) {
                  $(this).val(ui.item.locale);
                  return false;
              }, minLength: 1
          }).data("ui-autocomplete")._renderItem = function (ul, i) {
              return $("<li>").append("<a>" + i.locale + "</a>").appendTo(ul);
          };

         $(elements).find("#note-edit-location-site").autocomplete(
         {
                 source: function (request, response) {
                     $.get(self.appRoot + 'vin/SearchLocation',
                      {
                          c: self.null2str(self.model.country()),
                          r: self.null2str(self.model.region()),
                          l: self.null2str(self.model.location()),
                          lc: self.null2str(self.model.locale()),
                          s: request.term
                      },
                        function (result) { response(result); })
              },
              focus: function (event, ui) {
                  $(this).val(ui.item.site);
                  return false;
              },
              select: function (event, ui) {
                  $(this).val(ui.item.site);
                  return false;
              }, minLength: 1
          }).data("ui-autocomplete")._renderItem = function (ul, i) {
              return $("<li>").append("<a>" + i.site + "</a>").appendTo(ul);
          };



         $(elements).find("#note-edit-producer").autocomplete(
         {

             source: function (request, response) {
                 $.get(self.appRoot + 'vin/SearchProducer', { term: request.term },
                    function (result) {
                        response(result);
                    });
          },

          focus: function (event, ui) {
              $(this).val(ui.item.producer);
              return false;
          },
          select: function (event, ui) {
              $(this).val(ui.item.producer);

              return false;
          },
          minLength: 1
      }).data("ui-autocomplete")._renderItem = function (ul, i) {
          var c = null;
          c = i.label;
          return $("<li>").append("<a>" + i.producer + "</a>").appendTo(ul);
      };



        //
        // 1.use producer value in the filter
        // 2. fill variety, dryness, color when wine label selected
        //



     $(elements).find("#note-edit-label").autocomplete(
     {

         source: function (request, response) {
             $.get(self.appRoot + 'vin/SearchWineLabel', { term: request.term, p: self.model.producer },
                    function (result) {
                        response(result);
                    });
         },

         focus: function (event, ui) {
             $(this).val(ui.item.label);
             return false;
         },
         select: function (event, ui) {
             $(this).val(ui.item.label);

             self.model.color(ui.item.colorClass);
             self.model.variety(ui.item.variety);
             self.model.dryness(ui.item.dryness);

             self.model.country(ui.item.country);
             self.model.region(ui.item.region);
             self.model.location(ui.item.location);
             self.model.locale(ui.item.locale);
             self.model.site(ui.item.site);

             return false;
         },
         minLength: 1
     }).data("ui-autocomplete")._renderItem = function (ul, i) {
         var c = null;
         c = i.label;
         return $("<li>").append("<a>" + i.label + " (" + i.producer + ")</a>").appendTo(ul);
     };





        $(elements).find("#note-edit-variety").autocomplete(
        {
            source: self.appRoot + 'review/Variety',
            minLength: 3
        });


        $(elements).find("#note-edit-tastingDate").datepicker();

        //
        // set current year to start drink date
        //

        var year = (new Date()).getFullYear();

        //
        // disable it for now. 07.19.2014
        //
        //$(elements).find("#note-edit-drink-from").val(year);
        //model.drinkDateLo = "" + year;
    }

