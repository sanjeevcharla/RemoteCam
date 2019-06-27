import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Capture } from '../camera-view/Capture';
import { environment } from 'src/environments/environment';


@Injectable()
export class CameraService {
    constructor(private http: HttpClient) { }

    getCapture() {
        return this.http.get<Capture>(environment.apiUrl + "api/cam");
    }

}

