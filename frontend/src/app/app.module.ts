import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { UserInputComponent } from './user-input/user-input.component';

import { HttpClientModule } from '@angular/common/http';
import { MonthInputComponent } from './month-input/month-input.component';
import { CategoryInputComponent } from './category-input/category-input.component';

@NgModule({
  declarations: [
    AppComponent,
    UserInputComponent,
    MonthInputComponent,
    CategoryInputComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
