import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'client';
  http = inject(HttpClient);
  usres: any;


  ngOnInit(): void {
    this.http.get("http://localhost:5000/api/users").subscribe(
      {
        next: (data) => this.usres = data,
        error: (error) => console.log(error),
        complete:()=> console.log("completed")
      }

    );
  }
}
