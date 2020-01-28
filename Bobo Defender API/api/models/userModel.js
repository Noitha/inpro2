'use strict';

const mongoose = require('mongoose');
const crypto = require('crypto');
const Schema = mongoose.Schema;


const LevelSchema = new Schema({
    id:
        {
            type: Number,
        },
    crystals:
        {
            type: Number
        }
});

const UserSchema = new Schema({
    username:String,
    email:String,
    password: String,
    auth_token: String,
    salt: String,
    activeSkinId: Number,
    unlockedSkins: [Number],
    levels: {type:[LevelSchema], dropDups: false}
});

UserSchema.methods.SetPassword = function(password)
{
    this.salt = crypto.randomBytes(32).toString('hex');
    this.password = crypto.pbkdf2Sync(password, this.salt, 10000, 512, 'sha512').toString('hex');
};

UserSchema.methods.ValidatePassword = function(password)
{
    return this.password === crypto.pbkdf2Sync(password, this.salt, 10000, 512, 'sha512').toString('hex');
};

module.exports = mongoose.model('User', UserSchema);