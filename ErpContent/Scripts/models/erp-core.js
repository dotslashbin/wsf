



(function () {
    var window = this || (0, eval)('this');

    window['erp'] = {};

    erp.version = '1.0.0.0';
    erp.wsf_path = '/';

    erp.settings = {};

    erp.utils = {};


    //
    // 
    //
    erp.settings.waitCursor = true;

    erp.settings.isWaitCursorOn = function () {
        return erp.settings.waitCursor == true;
    };

    erp.settings.setWaitCursor = function (state) {
         erp.settings.waitCursor = state;
    };

    //
    //
    //
    erp.utils.Json2Date =  function(str) {


        // "/Date(-62135578800000)/" uninitilized date
        //
        //


        if (!str )
            return "";


        if (/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(str)) {
            //
            // it is already a good date str.
            //
            return str;
        }


        var d = new Date(parseInt(str.replace('/Date(', '')));

        if (d.getFullYear() < 1000)
            return "";



        return erp.utils.Date2Str(d);
    }

    // 
    // convert year string "2013" into date string "2/2/2013"
    // we use '2/2' avoid confusion if server time is ahead of client time and date in this case would
    // be one year behind
    //
    erp.utils.Year2ValidDate = function (str) {

        if (!str || str.length == 0) {
            return str;
        }

        return "2/2/" + str;
    }

    erp.utils.Json2Year = function (str) {


        if (!str )
            return "";


        if (/^\d{4}$/.test(str)) {
            //
            // it is already a good date str.
            //
            return str;
        }


        if (/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(str)) {
            //
            //  it is a full date, need only year
            //
            return  str.substr(str.length-4,4);
        }



        var d = new Date(parseInt(str.replace('/Date(', '')));

        if (d.getFullYear() < 1000)
            return "";

        return erp.utils.Date2Year(d);
    }



    //
    //
    //
    erp.utils.Date2Str = function (d) {
        if (!d)
            return "";
        return "" + (d.getMonth() + 1) + "/" + d.getDate() + "/" + d.getFullYear();
    }

    erp.utils.Date2Year = function (d) {
        if (!d)
            return "";
        return ""  + d.getFullYear();
    }



    //
    //
    //
    erp.utils.FindItemById = function (id, arr) {
        try {
            for (var i = 0; i < arr.length; i++) {
                if (arr[i].id == id) {
                    return arr[i];
                }
            }

        } catch (e) {

        }
        return null;
    }

    erp.utils.Str2Html = function (t) {

        if (t) {
            //var r = "<p>" + t + "</p>";
            var r = t;
            r = r.replace(/\r\n/g, "<br />").replace(/\n/g, "<br />");

            return r;
        }
        return "";
    }



    erp.utils.statusColor = function(state) {
        if (state >= 100) // EditorsCommon.WorkFlowState.PUBLISHED
            return 'black';

        if (state == 0)   // EditorsCommon.WorkFlowState.DRAFT
            return 'red';

        if (state == 10 ) // EditorsCommon.WorkFlowState.READY_FOR_REVIEW
            return 'orange';

        if (state == 60) // EditorsCommon.WorkFlowState.READY_APPROVED
            return 'blue';

        return 'black';
    }




    erp.utils.statusName = function (state) {

        if (state >= 100) // EditorsCommon.WorkFlowState.PUBLISHED
            return 'Published';

        if (state == 0)   // EditorsCommon.WorkFlowState.DRAFT
            return 'Draft';

        if (state == 10) // EditorsCommon.WorkFlowState.READY_FOR_REVIEW
            return 'Ready For Review/Approval';

        if (state == 60) // EditorsCommon.WorkFlowState.READY_APPROVED
            return 'Approved';

        return 'Unknown';
    }

    //
    // borrowed from http://stackoverflow.com/questions/4545311/download-a-file-by-jquery-ajax
    //
    erp.utils.ajaxDownload = function (url, data) {
        var $iframe,
            iframe_doc,
            iframe_html;

        if (($iframe = $('#download_iframe')).length === 0) {
            $iframe = $("<iframe id='download_iframe'" +
                        " style='display: none' src='about:blank'></iframe>"
                       ).appendTo("body");
        }

        iframe_doc = $iframe[0].contentWindow || $iframe[0].contentDocument;
        if (iframe_doc.document) {
            iframe_doc = iframe_doc.document;
        }

        iframe_html = "<html><head></head><body><form method='POST' action='" + url + "'>";

        Object.keys(data).forEach(function (key) {
            iframe_html += "<input type='hidden' name='" + key + "' value='" + data[key] + "'>";

        });

        iframe_html += "</form></body></html>";

        iframe_doc.open();
        iframe_doc.write(iframe_html);
        $(iframe_doc).find('form').submit();
    }



    //
    // add validation rule rating
    // idea borrowed at http://stackoverflow.com/questions/280759/jquery-validate-how-to-add-a-rule-for-regular-expression-validation
    //

    $.validator.addMethod(
        "rating",
         function (value, element) {
             var result = true;
             if (value) {

                 result = false;
                 result = /^[5-9][0-9]$/.test(value);
                 result = result || /^100$/.test(value);
                 result = result || /^[5-9][0-9]-[5-9][0-9]$/.test(value);
                 result = result || /^[5-9][0-9]-100$/.test(value);
             }
             return this.optional(element) || result;
         },
         "Must be more than 50 or range like 90-94"
         );

    //
    // add validation rule rating
    // idea borrowed at http://stackoverflow.com/questions/280759/jquery-validate-how-to-add-a-rule-for-regular-expression-validation
    //

    $.validator.addMethod(
        "vintage",
         function (value, element) {
             var result = true;
             if (value) {

                 result = false;
                 result = result || /^[1-9][0-9][0-9][0-9]$/.test(value);
                 result = result || /^NV$/.test(value);
             }
             return this.optional(element) || result;
         },
         "Vintage should be an Year like 2013 or NV"
         );




    //
    // borrowed at http://www.robsearles.com/2010/05/jquery-validate-url-adding-http/
    //
    jQuery.validator.addMethod("complete_url", function (value, element) {
        // if no url, don't do anything
        if (value.length == 0) { return true; }

        // if user has not entered http:// https:// or ftp:// assume they mean http://
        if (!/^(https?|ftp):\/\//i.test(value)) {
            value = 'http://' + value; // set both the value
            //$(elem).val(val); // also update the form element
        }

        // now check if valid url
        // http://docs.jquery.com/Plugins/Validation/Methods/url
        // contributed by Scott Gonzalez: http://projects.scottsplayground.com/iri/

        return this.optional(element) || /^(https?|s?ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i.test(value);
    },"Invalid URL");


}
)();