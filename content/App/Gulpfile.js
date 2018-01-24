/// <binding AfterBuild='copy-modules' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

"use strict";

var gulp = require('gulp'),
    clean = require('gulp-clean'),
    glob = require('glob');

var paths = {
    devModules: "../Modules/",
    hostModules: "./Modules/",
    hostWwwrootModules: "./wwwroot/modules/",
    hostSideMenu: "./Views/Shared/"
};

var modules = loadModules();

gulp.task('clean-module', function () {
    return gulp.src([paths.hostModules + '*', paths.hostWwwrootModules + '*'], { read: false })
        .pipe(clean());
});

gulp.task('copy-modules', ['clean-module'], function () {
    modules.forEach(function (module) {
        gulp.src(paths.devModules + module.fullName + '/Views/**/*.*')
            .pipe(gulp.dest(paths.hostModules + module.fullName+ '/Views'));
        gulp.src(paths.devModules + module.fullName +'/bin/Debug/netcoreapp2.0/**/*.*')
            .pipe(gulp.dest(paths.hostModules + module.fullName + '/bin'));
        gulp.src(paths.devModules + module.fullName + '/module.json')
            .pipe(gulp.dest(paths.hostModules + module.fullName));
        gulp.src(paths.devModules + module.fullName + '/Views/SideMenu/*.*')
            .pipe(gulp.dest(paths.hostSideMenu));
    });
});

gulp.task('copy-static-views', function () {
    modules.forEach(function (module) {
        gulp.src(paths.devModules + module.fullName + '/Views/**/*.*')
            .pipe(gulp.dest(paths.hostModules + module.fullName + '/Views'));
        gulp.src(paths.devModules + module.fullName + '/Views/SideMenu/*.*')
            .pipe(gulp.dest(paths.hostSideMenu));
    });

});

gulp.task('copy-static', function () {
    modules.forEach(function (module) {
        gulp.src(paths.devModules + module.fullName + '/Views/**/*.*')
            .pipe(gulp.dest(paths.hostModules + module.fullName + '/Views'));
        gulp.src(paths.devModules + module.fullName + '/module.json')
            .pipe(gulp.dest(paths.hostModules + module.fullName));
        gulp.src(paths.devModules + module.fullName + '/Views/SideMenu/*.*')
            .pipe(gulp.dest(paths.hostSideMenu));
    });
});

function loadModules() {
    var moduleManifestPaths,
        modules = [];

    moduleManifestPaths = glob.sync(paths.devModules + '*.*/module.json', {});
    moduleManifestPaths.forEach(function (moduleManifestPath) {
        var moduleManifest = require(moduleManifestPath);
        modules.push(moduleManifest);
    });

    return modules;
}