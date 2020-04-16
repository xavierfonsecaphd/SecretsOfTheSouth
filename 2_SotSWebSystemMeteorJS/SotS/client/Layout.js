Template.Layout.onCreated (function() {
    
});

Template.Layout.onRendered( function() {
  this.subscribe( 'trash', function() {
      $( ".preloader" ).delay( 1000 ).fadeOut( 'slow', function() {
        $( ".loading-wrapper" ).fadeIn( 'slow' );
      });
    });
  
});

Template.Layout.helpers({
  
}); 

Template.Layout.events({ 
  'click .jump-about': function() { 
      // famos fazer o set duma variável de sessão
      //Session.set('newRecipe', true);
      FlowRouter.go('about-research');
 }
}); 

