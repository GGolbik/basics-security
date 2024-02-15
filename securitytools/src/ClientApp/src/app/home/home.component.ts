import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FragmentParams } from '../services/models';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  params = new FragmentParams();

  constructor(private router: Router, private route: ActivatedRoute) {
    
  }
  
  ngOnInit(): void {
    this.params = FragmentParams.parse(this.route);
  }

  selectedIndexChange(value:any) {
    this.params.tabIndex = value;
    this.params.stepperIndex = 0;
    this.params.updateUrl(this.router);
  }
}
