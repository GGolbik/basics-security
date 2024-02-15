import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrivatekeyComponent } from './privatekey.component';

describe('PrivatekeyComponent', () => {
  let component: PrivatekeyComponent;
  let fixture: ComponentFixture<PrivatekeyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PrivatekeyComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrivatekeyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
