import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-company-register-layout',
  imports: [RouterOutlet],
  templateUrl: './company-register-layout.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CompanyRegisterLayoutComponent {}

