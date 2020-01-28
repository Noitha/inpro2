'use strict';

const mongoose = require('mongoose');
const User = mongoose.model('User');
require('dotenv').config();
const jwt = require('jsonwebtoken');

//Register user | add a new user to the database.
exports.RegisterUser = async function (req, res)
{
    //Get the registration-data sent by the game and parse it through JSON into an object.
    let registrationData = JSON.parse(req.body.registerData);

    //Create a response-object.
    let responseObject =
        {
            email:
                {
                    isValid: true,
                    message: "",
                },
            username:
                {
                    isValid: true,
                    message: "",
                },
            password:
                {
                    isValid: true,
                    message: "",
                },
            passwordRetype:
                {
                    isValid: true,
                    message: "",
                },
            successful: true
        };

    //Define an email-regex-validation-pattern.
    const emailRegex = /\S+@\S+\.\S+/;

    //Validate the data received from the user and response with clear messages.

    //Email Validation
    if (registrationData.email.length === 0)
    {
        responseObject.email = {isValid : false, message: "Email is required."};
        responseObject.successful = false;
    }
    else if (!emailRegex.test(registrationData.email))
    {
        responseObject.email = {isValid : false, message: "Email format is not valid."};
        responseObject.successful = false;
    }
    else
    {
        await User.find({ email: registrationData.email }, function (_, entries)
        {
            if (entries.length > 0)
            {
                responseObject.email = {isValid : false, message: "Email already registered."};
                responseObject.successful = false;
            }
            else
            {
                responseObject.email = {isValid: true, message: "Email is valid."};
            }
        });
    }

    //Username validation
    if (registrationData.username.length === 0)
    {
        responseObject.username = {isValid : false, message: "Username is required."};
        responseObject.successful = false;
    }
    else if (registrationData.username.length <= 2)
    {
        responseObject.username = {isValid : false, message: "Username must be at least 3 characters long."};
        responseObject.successful = false;
    }
    else if(registrationData.username.length > 15)
    {
        responseObject.username = {isValid : false, message: "Username cannot have more than 15 characters."};
        responseObject.successful = false;
    }
    else
    {
        await User.find({ username: registrationData.username }, function (_, entries)
        {
            if (entries.length > 0)
            {
                responseObject.username = {isValid : false, message: "Username is taken."};
                responseObject.successful = false;
            }
            else
            {
                responseObject.username = {isValid: true, message: "Username is available."};
            }
        });
    }

    //Password validation
    if (registrationData.password.length === 0)
    {
        responseObject.password = {isValid : false, message: "Password cannot be blank."};
        responseObject.successful = false;
    }
    else if (registrationData.password.length < 5)
    {
        responseObject.password = {isValid : false, message: "Password must be at least 6 characters long."};
        responseObject.successful = false;
    }
    else
    {
        responseObject.password = {isValid : true, message: "Valid."};
    }

    //Password-match validation
    if (registrationData.password !== registrationData.passwordRetype)
    {
        responseObject.passwordRetype = {isValid : false, message: "Passwords are not matching."};
        responseObject.successful = false;
    }
    else
    {
        responseObject.passwordRetype = {isValid : true, message: "Matching."};
    }

    //If any validation has failed, send the registerErrorMessage-object.
    if (responseObject.successful)
    {
        let newUser = new User
        (
            {
                email: registrationData.email,
                username: registrationData.username,
                activeSkinId: 1,
                unlockedSkins: [1,2,3],
                levels: [],
                auth_token: jwt.sign({username : registrationData.username}, process.env.JWT_KEY)
            }
        );

        newUser.SetPassword(registrationData.password);

        await newUser.save(function (error)
        {
            if (error)
            {
                console.log(error);
                responseObject.successful = false
            }
            res.send(responseObject);
        });
    }
    else
    {
        res.send(responseObject);
    }
};

//Login user | authenticate user.
exports.LoginUser = async function (req, res)
{
    //Get the login-data sent by the game and parse it through JSON into an object.
    const loginData = JSON.parse(req.body.loginData);

    //Create a response-object.
    let responseObject =
        {
            username:
                {
                    isValid: true,
                    message: "",
                },
            password:
                {
                    isValid: true,
                    message: "",
                },
            userData : User,
            successful: true
        };

    //Validate the data received from the user and response with clear messages.

    //Username validation
    if (loginData.username.length === 0)
    {
        responseObject.username = {isValid : false, message: "Username is required."};
        responseObject.successful = false;

        if (loginData.password.length === 0)
        {
            responseObject.password = {isValid : false, message: "Password cannot be blank."};
            responseObject.successful = false;
        }
    }
    else
    {
        await User.find({ username: loginData.username }, function (_, entries)
        {
            if (entries.length > 0)
            {
                responseObject.username = {isValid : true, message: "Username found."};

                //Password validation
                if(entries[0].ValidatePassword(loginData.password))
                {
                    entries[0].auth_token = jwt.sign({username : loginData.username}, process.env.JWT_KEY);
                    entries[0].save();
                    responseObject.userData = entries[0];
                }
                else
                {
                    responseObject.password = {isValid : false, message: "Wrong password."};
                    responseObject.successful = false;
                }
            }
            else
            {
                responseObject.username = {isValid: false, message: 'Username not found.'};
                responseObject.successful = false;

                if (loginData.password.length === 0)
                {
                    responseObject.password = {isValid : false, message: "Password cannot be blank."};
                    responseObject.successful = false;
                }
            }
        });
    }

    res.send(responseObject);
};

//Validate Token | Check expiration.
exports.ValidateToken = async function (req, res)
{
    //Get the username sent by the game and parse it through JSON into an object.
    const data = JSON.parse(req.body.username);

    await User.find({ username: data.username }, function (_, entries)
    {
        if (entries.length > 0)
        {
            //In case if logged-out an empty string is stored as the token.
            if(entries[0].auth_token === "")
            {
                res.send(null);
                return;
            }
           
            const iat = jwt.decode(entries[0].auth_token).iat;

            //604800 = 7 days.
            if(iat + 604800 > Date.now() / 1000)
            {
                entries[0].auth_token = jwt.sign({username : data.username}, process.env.JWT_KEY);
                entries[0].save();
                res.send(entries[0]);
            }
            else
            {
                res.send(null);
            }
        }
        else
        {
            res.send(null);
        }
    });
};

//Change the skin of the player.
exports.ChangeSkin = async function (req, res)
{
    //Get the username & new active skin id sent by the game and parse it through JSON into an object.
    const data = JSON.parse(req.body.changeSkin);

    await User.find({ username: data.username }, function (_, entries)
    {
        if (entries.length > 0)
        {
            entries[0].activeSkinId = data.activeSkinId;
            entries[0].save();
            res.send(true);
        }
        else
        {
            res.send(false);
        }
    });
};

//Logout.
exports.Logout = async function (req, res)
{
    //Get the username sent by the game and parse it through JSON into an object.
    const data = JSON.parse(req.body.logout);
    
    await User.find({ username: data.username }, function (_, entries)
    {
        if (entries.length > 0)
        {
            entries[0].auth_token = "";
            entries[0].save();
            res.send(true);
        }
        else
        {
            res.send(false);
        }
    });
};

//Get level information.
exports.GetLevelInfo = async function (req, res)
{
    //Get the username sent by the game and parse it through JSON into an object.
    const data = JSON.parse(req.body.levelInfo);

    await User.find({ username: data.username }, function (_, entries)
    {
        if (entries.length > 0)
        {
            res.send(entries[0].levels);
        }
        else
        {
            res.send(null);
        }
    });
};

//Update level score.
exports.UpdateLevelScore = async function (req, res) 
{
    //Get the username sent by the game and parse it through JSON into an object.
    const data = JSON.parse(req.body.updateLevelScore);
    
    await User.find({ username: data.username}, function (_, entries)
    {
        if (entries.length > 0)
        {
            var found = false;
            
            //Check if once completed and update entry.
            for(var i = 0; i < entries[0].levels.length; i++)
            {
                if(entries[0].levels[i].id === parseInt(data.levelId))
                {
                    found = true;
                    
                    //Only update if score is greater than previous.
                    if(data.newCrystalScore > entries[0].levels[i].crystals)
                    {
                        entries[0].levels[i].crystals = data.newCrystalScore;
                        entries[0].save();
                    }

                    break;
                }
            }
            
            //Add level to array if not found.
            if(!found)
            {
                entries[0].levels.push({id: data.levelId, crystals: data.newCrystalScore});
                entries[0].save();
            }

            if(data.skinId !== 0)
            {
                entries[0].unlockedSkins.push(parseInt(data.skinId));
            }
            
            res.send(entries[0]);
        }
        else
        {
            res.send(null);
        }
    });
};