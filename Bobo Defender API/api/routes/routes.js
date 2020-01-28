'use strict';

module.exports = function(app)
{
    const controller = require('../controllers/controller');
    
    app.route('/register')
        .post(controller.RegisterUser);

    app.route('/login')
        .post(controller.LoginUser);

    app.route('/validateToken')
        .post(controller.ValidateToken);

    app.route('/logout')
        .post(controller.Logout);
    
    app.route('/changeSkin')
        .post(controller.ChangeSkin);
    
    app.route('/getLevelInfo')
        .post(controller.GetLevelInfo);
    
    app.route('/updateLevelScore')
        .post(controller.UpdateLevelScore);
};