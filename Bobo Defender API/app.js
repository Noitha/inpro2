'use strict';

//Import packages & models.
const mongoose = require('mongoose');
const express = require('express');
const bodyParser = require('body-parser');
require('dotenv').config();
require('./api/models/userModel');

//Import the route for authentication.
const authenticationRoute = require('./api/routes/routes');

//Create an app instance.
const app = express();

//Select port.
const port = process.env.port || 3000;

//Connect to mongo-db.
mongoose.connect(process.env.MONGO_DB, {useNewUrlParser: true, useUnifiedTopology: true, useCreateIndex: true});

//Add middleware.
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

//Register the route.
authenticationRoute(app);

app.listen(port, () => console.log("Server is listening on port:" + port));