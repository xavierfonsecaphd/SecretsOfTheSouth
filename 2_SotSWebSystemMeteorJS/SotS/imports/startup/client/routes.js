import { FlowRouter } from 'meteor/kadira:flow-router';
import { BlazeLayout } from 'meteor/kadira:blaze-layout';

// Import needed templates
import '../../ui/layouts/body/body.js';
import '../../ui/pages/home/home.js';
import '../../ui/pages/not-found/not-found.js';


FlowRouter.route('/', {
  name: 'home',
  action(){
      // if the user is logged in
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main:'Home'});
  }
});
FlowRouter.route('/about-research', {
  name: 'about-research',
  action(){
      // if the user is logged in
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main: 'AboutResearch'});
  }
});


FlowRouter.route('/HandleQRCode/:_id', {
  name: 'HandleQRCode',
  action(){
      //GAnalytics.pageview();
      FlowRouter.go('/');
  }
});

FlowRouter.route('/Admin', {
  name: 'Admin',
  action(){
      // if the user is logged in
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main: 'Admin'});
  }
});

FlowRouter.route('/Challenges', {
  name: 'Challenges',
  action(){
      // if the user is logged in
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main: 'Challenges'});
  }
});


FlowRouter.route('/Challenges/:_id', {
  name: 'Challenge',
  action(){
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main: 'Challenge'});
  }
});

FlowRouter.route('/TermsOfServiceGameApp', {
  name: 'TermsOfServiceGameApp',
  action(){
      // if the user is logged in
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main: 'TermsOfService'});
  }
});
FlowRouter.route('/PrivacyPolicy', {
  name: 'PrivacyPolicy',
  action(){
      // if the user is logged in
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main: 'PrivacyPolicy'});
  }
});
FlowRouter.route('/QRCode', {
  name: 'QRCode',
  action(){
      //GAnalytics.pageview();
      BlazeLayout.render('QRCode');
  }
});
FlowRouter.route('/LoginQRCodeCredentials/:_id', {
  name: 'LoginQRCodeCredentials',
  action(){
      //GAnalytics.pageview();
      BlazeLayout.render('LoginQRCodeCredentials');
  }
});
FlowRouter.route('/PrintLoginQRPage', {
  name: 'PrintLoginQRPage',
  action(){
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main: 'PrintLoginQRPage'});
  }
});
FlowRouter.route('/playerData', {
  name: 'playerData',
  action(){
      // if the user is logged in
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main: 'PlayerData'});
  }
});

// google5e909b4ba57e88dd
FlowRouter.route('/google5e909b4ba57e88dd.html', {
  name: 'googleVerification',
  action(){
      // if the user is logged in
      //GAnalytics.pageview();
      BlazeLayout.render('Layout', {main: 'google5e909b4ba57e88dd'});
  }
});

FlowRouter.notFound = {
  action() {
    BlazeLayout.render('App_body', { main: 'App_notFound' });
  },
};


Accounts.onLogin(function () {
  FlowRouter.go('Challenges');
});

Accounts.onLogout(function () {
  FlowRouter.go('/');
});


