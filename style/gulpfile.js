//ş
'use strict';

const gulp = require('gulp'),
      del = require('del'),
      sass = require('gulp-sass');

// # CSS
gulp.task('css.clean', function () {
    return del('./md.css');
});

gulp.task('css.pack', ['css.clean'], function () {
    return gulp.src('./sass/md.scss')
        .pipe(sass.sync({ precision: 8, outputStyle: 'compressed' }).on('error', sass.logError))
        .pipe(gulp.dest('./'));
});

gulp.task('default', ['css.pack']);
