import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  
  title = 'Dating App';
  users: any;

  constructor(private http: HttpClient){}

  ngOnInit() {
    this.getUsers();
  }

  getUsers(){
    this.http.get('http://localhost:5182/api/Users').subscribe(res => {
      this.users = res;
    });
  }

}
