import { Challenges_VotingDB } from '/Collections/Challenges_Voting.js';
import { SotSEventsDB } from '/Collections/SotSEvents.js';
// this is info to create streams to send files back
// https://www.npmjs.com/package/form-data
// this can be useful to stream the images back to the games

/*
// http://localhost:3000/posts/123
JsonRoutes.add("get", "/posts/:id", function (req, res, next) {
    var idPam = req.params.id;
  
    JsonRoutes.sendResult(res, {
      data: "Great Stuff " + idPam
    });
  });*/


// *********************************************************************
// List all the pics in a voting challenge ID folder
// http://localhost:3000/api/listphotosofchallenge/CMCY8NAxEDRigaJ2f
// *********************************************************************
JsonRoutes.add("get", "/api/listphotosofchallenge/:votingchallengeID", function (req, res, next) {
      var challengeID = req.params.votingchallengeID; 
      var succeeded = true;
      var errMessage;
      //const busboy = new Busboy({headers: req.headers});
      var fs       = Npm.require("fs");
      
      // first, defining the correct path of the challenge
      var path;
      if (process.env.NODE_ENV === "development") {
          path = process.env["PWD"] + "/public/VotingChallenges/" + challengeID + "/";
      }
      else {
        if (!fs.existsSync("/home/fdossantosfons/meteorProjects/deployed/bundle/programs/web.browser/app/VotingChallenges/")) {
          succeeded = false;// throw err; 
          errMessage = "There are no pictures in the challenge. Please create your own, and upload it first";
        }

        path = "/home/fdossantosfons/meteorProjects/deployed/bundle/programs/web.browser/app/VotingChallenges/"+ challengeID + "/";
      }

      if (req.method === "GET") { 
      
        var images = fs.readdirSync(path).filter(
          function(i) {
            //console.log('File in directory: ', i);
            
            if (i.split('_').length == 2)
            {
              if (i.includes('.png')) {
                return i;
              }
              else {
              //  console.log('Not included this file ', i, ', because it does not seem to be a picture! ');
              }
            }
            else {
              //console.log('Not included this file ', i, ', because the number of words is ', i.split('_').length);
            }
            
          }
        );
        
        if (succeeded === true) {
          JsonRoutes.sendResult(res, {
            status: 200,
            data: images
          });
        }
      
      }
      else {
        // if it actually failed, then send feedback
        if (succeeded === false) {
          JsonRoutes.sendResult(res, {
            status: 500,
            data: errMessage
          });
        }
        
        // move onto another part of the request
        next();
      }
});

// downloadimage
JsonRoutes.add("get", "/api/downloadimage/:votingchallengeIDAndFileName", function (req, res, next) {
    var challengeID_filename = req.params.votingchallengeIDAndFileName; // votingchallengeID
    //console.log ('received: ',req.params.votingchallengeIDAndFileName);
    var partsOfStr = challengeID_filename.split('_');
    //console.log ('number of split tokens: ',partsOfStr.length);
    var challengeID = partsOfStr[0];  // votingchallengeID
    var tmpfilename = partsOfStr[1] + '_' + partsOfStr[2];     // filename
    //console.log ('filename before: ',tmpfilename);
    
    var filename = unescape(tmpfilename);
    filename = filename.replace(new RegExp('kzktsz', 'g'), ' ');
    //console.log ('filename after: ',filename);
    
    var succeeded = true;
    var errMessage;

    //const busboy = new Busboy({headers: req.headers});
    var fs     = Npm.require("fs");
  // first, defining the correct path of the challenge
    var path;
    if (process.env.NODE_ENV === "development") {
        path = process.env["PWD"] + "/public/VotingChallenges/" + challengeID + "/";
    }
    else {
      if (!fs.existsSync("/home/fdossantosfons/meteorProjects/deployed/bundle/programs/web.browser/app/VotingChallenges/")) {
        succeeded = false;// throw err; 
        errMessage = "There are no pictures in the challenge. Please create your own, and upload it first";
      }

      path = "/home/fdossantosfons/meteorProjects/deployed/bundle/programs/web.browser/app/VotingChallenges/"+ challengeID + "/";
    }

    if (req.method === "GET") {
      // does the file exist in the server?
      fs.access(path+filename, (err) => {
        if (err) {
          succeeded = false;// throw err; 
          errMessage = "File " + filename + " does not seem to exist: "+err;
        }
      });

      var data = fs.readFileSync(path+filename);
        res.writeHead(200, {
                'Content-Type': 'image'
            });
        res.write(data);
        res.end();

      /*
      let readStream = fs.createReadStream(path+filename);
      let chunks = [];
    
      // Listen for data
      readStream.on('data', chunk => {
        console.log('Localy read data: ', chunk.length);
        chunks.push(chunk);
      });
    
      // File is done being read
      // 'end' instead?
      readStream.on('close', () => {
        // Create a buffer of the image from the stream
        //return cb(null, Buffer.concat(chunks));
          //res.setHeader('content-type', 'application/json');
          //res.set("Content-Disposition","attachment; filename=" + filename);
          //res.send(Buffer.concat(chunks));
          JsonRoutes.sendResult(res, {
            data: Buffer.concat(chunks),
            headers: {"Content-Disposition":"attachment; filename=" + filename}
          });
          
      });*/

    }
    else {
      // if it actually failed, then send feedback
      if (succeeded === false) {
        JsonRoutes.sendResult(res, {
          status: 500,
          data: errMessage
        });
      }
      
      // move onto another part of the request
      next();
    }
});
// *********************************************************************
// NOT FINISHED. DELETE!
// Receive a list of images to download in the challenge id folder:
// localhost:3000/api/downloadPhotosofchallenge/CMCY8NAxEDRigaJ2f
// *********************************************************************
JsonRoutes.add("post", "/api/downloadPhotosofchallenge/:votingchallengeID", function (req, res, next) {
      var challengeID = req.params.votingchallengeID; 
      var succeeded = true;
      var errMessage;
      const busboy = new Busboy({headers: req.headers});
      var fs       = Npm.require("fs");
      var path;

      if (process.env.NODE_ENV === "development") {
          path = process.env["PWD"] + "/public/VotingChallenges/" + challengeID + "/";
      }
      else {
        if (!fs.existsSync("/home/fdossantosfons/meteorProjects/deployed/bundle/programs/web.browser/app/VotingChallenges/")) {
          succeeded = false;// throw err; 
          errMessage = "There are no pictures in the challenge. Please create your own, and upload it first";
        }

        path = "/home/fdossantosfons/meteorProjects/deployed/bundle/programs/web.browser/app/VotingChallenges/"+ challengeID + "/";
      }

      if (req.method === "POST") { 

        busboy.on('file', function(fieldname, file, filename, encoding, mimetype) {
          console.log('File [' + fieldname + ']: filename: ' + filename + ', encoding: ' + encoding + ', mimetype: ' + mimetype);
          // {list:[A9C1620AF8977C96_greatstuff,hein?.png,B9C1620AF8977C96_Iknowwhoyouare.png,C9C1620AF8977C96_Mygod.png,D9C1620AF8977C96_Margarida.png,E9C1620AF8977C96_Hendrik.png,F9C1620AF8977C96_Meredith.png,G9C1620AF8977C96_Lol.png,I9C1620AF8977C96_Carl.png,X9C1620AF8977C96_lolita.png]}
          var a = filename.indexOf('[') + 1;
          var b = filename.indexOf(']');
          var result = filename.substring(a, b);
          console.log('Result : ' + result);
          var str = result.replace(new RegExp('kzktsz', 'g'), ' ');
          var list = str.split("eof,");

          for (var k = 0; (k < list.length) && succeeded; k++) {
            console.log('Element : ' + list[k]);

            // just guaranteeing that the file actually exists on disk after all the parsing. 
            // if not, it quits now.
            fs.access(path+list[k], (err) => {
              if (err) {
                succeeded = false;// throw err; 
                errMessage = "File " + list[k] + " does not seem to exist: "+err;
              }
            });



            let readStream = fs.createReadStream(path+list[k]);
            let chunks = [];
    
            // Listen for data
            readStream.on('data', chunk => {
              console.log('Localy read data: ', chunk.length);
              //chunks.push(chunk);
              
              JsonRoutes.sendResult(res, {
                data: chunk
              });
            });
          
            // File is done being read
            // 'end' instead?
            readStream.on('close', () => {
              // Create a buffer of the image from the stream
              //return cb(null, Buffer.concat(chunks));
                //res.setHeader('content-type', 'application/json');
                //response.set("Content-Disposition","attachment; filename=" + list[k]);
                //response.send(Buffer.concat(chunks));
                
                
            });

            
          }
        });

        if (succeeded === true) {
            JsonRoutes.sendResult(res, {
                data: succeeded
              });
        }
        else {
            JsonRoutes.sendResult(res, {
                data: errMessage
              });
        }
        
        req.pipe(busboy);

      }
      else {
        // if it actually failed, then send feedback
        if (succeeded === false) {
          JsonRoutes.sendResult(res, {
            status: 500,
            data: errMessage
          });
        }
        
        // move onto another part of the request
        next();
      }
});
/*
JsonRoutes.add("get", "/api/mypdw", function (req, res, next) {
  var fs     = Npm.require("fs");
  
  JsonRoutes.sendResult(res, {
    data: process.env["PWD"]
  });

});*/

// emailopenquizanswer
JsonRoutes.add("post", "/api/emailopenquizanswer", function (req, res, next) { 

  var to = 'yourEmail@emailProvider.com';
  var from = 'secretsofthesouth@outlook.com';
  var subject = 'You have a new Open Quiz answer, from the SotS game';
  var text = req.body["message"];

  Email.send({ to, from, subject, text });

  if ((req.method === "POST")) { 
    JsonRoutes.sendResult(res, {
      data: req.body["message"]
    });
  }
});


// logeventfromgame
JsonRoutes.add("post", "/api/logeventfromgame", function (req, res, next) { 

  var name = req.body["name"];
  var playfabid = req.body["playfabid"];
  var latitude = req.body["latitude"];
  var longitude = req.body["longitude"];
  var timestamp = req.body["timestamp"];
  var message = req.body["message"];

  var ret = SotSEventsDB.insert({name, playfabid, latitude, longitude, timestamp, message});
  /*Meteor.call('updateChallenge_Voting', element._id, element.name, element.description, 
              element.ownerPlayFabID, element.typeOfChallengeIndex, element.latitude, element.longitude,
              element.task, element.imageURL, updatedListOfVotings, element.validated,
                  (error) => {
                      if (error) {
                          //return "Could not update player last location";
                          console.log("Could not update the List<string> of votings for this challenge: " + error);
                          succeeded = false;
                          return false;
                      } else {
                        console.log("Voting Challenge with ID " + challengeid + " updated Successfully.");
                        succeeded = true;
                        return true;
                      }
                  });*/

  if ((req.method === "POST")) { 
    JsonRoutes.sendResult(res, {
      data: ret
    });
  }
});




// *********************************************************************
// Receive a picture from the game, and store it under the identified
// challenge ID. It receives the challenge ID, and playfabID posting the 
// pic.
// http://localhost:3000/api/uploadphoto/CMCY8NAxEDRigaJ2f_A9C1620AF8977C96
// *********************************************************************
  JsonRoutes.add("post", "/api/uploadphoto/:votingchallengeIDAndPlayFabID", function (req, res, next) {
    var challengeID_playfabid = req.params.votingchallengeIDAndPlayFabID; // votingchallengeID
    var partsOfStr = challengeID_playfabid.split('_');
    
    var challengeID = partsOfStr[0]; // votingchallengeID
    var succeeded = true;
    var errMessage;

    const busboy = new Busboy({headers: req.headers});
    var fs     = Npm.require("fs");
    var os     = Npm.require("os");

    //var path = process.env["PWD"] + "/VotingChallenges/" + challengeID + "/";
    //var meteorRoot = fs.realpathSync( process.cwd() + '/../' );
    var path;
    if (process.env.NODE_ENV === "development") {
      path = process.env["PWD"] + "/public/VotingChallenges/" + challengeID + "/";
    }
    else {
      try {
          if (!fs.existsSync(process.env["PWD"] + "/programs/web.browser/app/VotingChallenges/")) {
            // If folder VotingChallenges does not exist, then set it up
            fs.mkdir(process.env["PWD"] + "/programs/web.browser/app/VotingChallenges/", { recursive: false }, (err) => {
                if (err) {
                  succeeded = false;// throw err; 
                  errMessage = err;
                }
              });
        }
      }
      catch (err) {
        succeeded = false;
        errMessage = err;
      }

      path = process.env["PWD"] + "/programs/web.browser/app/VotingChallenges/"+ challengeID + "/";
    }


    if ((req.method === "POST") && (succeeded)) {

        // If folder of this challenge does not exist, then create it
        try {
            if (!fs.existsSync(path)) {
                fs.mkdir(path, { recursive: false }, (err) => {
                    if (err) {
                      succeeded = false;// throw err; 
                      errMessage = err;
                    }
                  });
            }
            // now, I want to look at all the files in the selected folder, and I want to double check that
            // the player only sent one picture to it
            var playfabID = partsOfStr[1];
            var images = fs.readdirSync(path).filter(
              function(i) {
                console.log('File in directory: ', i);
                return i.includes(playfabID);
              }
            );
            images.forEach(function(i) { 
              
              
              fs.unlinkSync(path+i, function (err) {
                if (err) throw err;
                // if no error, file has been deleted successfully
                console.log('Deleted file: ', i , ' in path: ' + path);
            });  
            
            });
        }
        catch(err) {
            succeeded = false;
            errMessage = err;
        }

        
    
    if (succeeded === true)
    {
        //// files will be avaliable in request context in endpoints
        req.files = [];
        busboy.on('file', function(fieldname, file, filename, encoding, mimetype){
            const uploadedFile = {
            filename,
            mimetype,
            encoding,
            fieldname,
            data: null
            };
            
            console.log('busboy have file...', uploadedFile);
            //filename: 'photo.png',
            //mimetype: 'image/png',
            //encoding: '7bit',
            //fieldname:'file',
            //data: null 
            const buffers = [];
    
            file.on('data', function(data){
            console.log('data: ', data.length);
            buffers.push(data);
            });
            file.on('end', function() {
            console.log('EOF');
            uploadedFile.data = Buffer.concat(buffers);
            req.files.push(uploadedFile);
            fs.writeFileSync(path + filename, uploadedFile.data, { encoding: 'latin1'},
                    function (err) {
                        if (err) {succeeded = false; errMessage = err;} // //throw err;
                        else {succeeded = true;}
                    });
            });
        });

        if (succeeded === true) {
            // now, verify if the picture you just uploaded already has some kind of vote in the DB.
            // if so, please delete the registry for that picture
            // challengeID
            var list = Challenges_VotingDB.find({_id:challengeID});
            list.forEach(element => 
            { 
                var listOfVotings = element.listOfImagesAndVotes;
                if (listOfVotings) { 
                    // for every list of votes in the challenge, try to find a registry for the 
                    // picture you just inserted. If you find, delete it
                    var changed = false;
                    var updatedListOfVotings = [];
                    var tmpPlayFabID;
                    for (var i = 0; i < listOfVotings.length; i++) { 
                        // per voting done, grab the playfabid, which is the prefix of the files in the server
                        tmpPlayFabID = listOfVotings[i].split('_')[0];  
                        if (new String(tmpPlayFabID).valueOf() == new String(playfabID).valueOf()) {
                          // in this case you found a registry. Exclude it from the result by not including it
                          changed = true;
                        } else {
                            // if they're not the same, keep them
                            updatedListOfVotings.push(listOfVotings[i]);
                        }
                    }
                    // update the DB in case there was a change in the votes
                    if (changed) {
                        // In here, I can commit the change done to the listOfVotings
                        Meteor.call('updateChallenge_Voting', element._id, element.name, element.description, 
                        element.ownerPlayFabID, element.typeOfChallengeIndex, element.latitude, element.longitude,
                        element.task, element.imageURL, updatedListOfVotings, element.route, element.validated,
                            (error) => {
                                if (error) {
                                    //return "Could not update player last location";
                                    console.log("Could not update the List<string> of votings for this challenge: " + error);
                                    succeeded = false;
                                    return false;
                                } else {
                                  console.log("Voting Challenge with ID " + challengeID + " updated Successfully.");
                                  succeeded = true;
                                  return true;
                                }
                            });
                    }
                    
                }
            });


            JsonRoutes.sendResult(res, {
                data: succeeded
              });
        }
        else {
            JsonRoutes.sendResult(res, {
                data: errMessage
              });
        }
        
          req.pipe(busboy);
    }
    else {
        JsonRoutes.sendResult(res, {
        data: errMessage
    });}
    }
    else {
      if (succeeded === false) {
        JsonRoutes.sendResult(res, {
          data: errMessage
        });
      }
              next();
    }
  });

  function cleanPath(str) {
    if (str) {
      return str.replace(/\.\./g,'').replace(/\/+/g,'').
        replace(/^\/+/,'').replace(/\/+$/,'');
    }
  }
  function cleanName(str) {
    return str.replace(/\.\./g,'').replace(/\//g,'');
  }

  
  