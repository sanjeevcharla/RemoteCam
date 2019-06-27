import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CameraViewComponent } from './camera-view/camera-view.component';
import { CameraService } from './camera-view/camera-service';

@NgModule({
  declarations: [
    AppComponent,
    CameraViewComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [CameraService],
  bootstrap: [AppComponent]
})
export class AppModule { }
