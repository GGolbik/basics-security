import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BasicConstraintsComponent } from './basic-constraints.component';

describe('BasicConstraintsComponent', () => {
  let component: BasicConstraintsComponent;
  let fixture: ComponentFixture<BasicConstraintsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BasicConstraintsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BasicConstraintsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
