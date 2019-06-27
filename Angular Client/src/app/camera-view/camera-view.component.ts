import { Component, OnInit } from '@angular/core';
import { Capture } from './capture';
import { CameraService } from './camera-service';
import { DomSanitizer } from '@angular/platform-browser';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'camera-view',
  templateUrl: './camera-view.component.html',
  styleUrls: ['./camera-view.component.css']
})
export class CameraViewComponent implements OnInit {

  capture: Capture
  private _hubConnection: HubConnection;

  isCaptureAvailable() {
    return this.capture != null
  }
  constructor(private cameraService: CameraService, public domSanitizationService: DomSanitizer) { }

  ngOnInit() {

    this._hubConnection = new HubConnectionBuilder()
      .withUrl(environment.apiUrl + "cam")
      .build();
    this._hubConnection
      .start()
      .then(() => console.log('Connection started!'))
      .catch(err => console.log('Error while establishing connection :('));

    this._hubConnection.on('capture', () => {
      console.log('Retrieve capture')
      this.cameraService.getCapture().subscribe((data: Capture) => {
        this.capture = data
        this.capture.imageUrl = this.domSanitizationService
          .bypassSecurityTrustResourceUrl('data:image/jpg;base64,' + this.capture.image64);
      });
    });


  }

}
