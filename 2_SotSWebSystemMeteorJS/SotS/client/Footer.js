Template.Footer.events ({
    'click .btn': function() { 
        
        var parentOpener = window.opener; 
        window.opener = null; 
        window.open("/QRCode","_blank"); 
        window.opener = parentOpener; 
        parentOpener.focus();

        /*
        
        var windowObjectReference;
        var strWindowFeatures = "menubar=yes,location=no,resizable=yes,scrollbars=yes,status=yes";


        windowObjectReference = window.open("http://www.cnn.com/", "_blank", strWindowFeatures);
        
        */

   }
});