import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { DecimalPipe, DatePipe } from '@angular/common';

interface Transaction {
  id: string;
  amount: number;
  currency: string;
  merchantName: string;
  description: string;
  transactionDate: string;
  type: string;
}

@Component({
  imports: [RouterLink, DecimalPipe, DatePipe],
  selector: 'app-transactions',
  styleUrl: './transactions.css',
  templateUrl: './transactions.html',
})
export class TransactionsComponent implements OnInit {

  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private router = inject(Router);

  transactions = signal<Transaction[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  ngOnInit() {
    this.http.get<Transaction[]>('/api/transactions').subscribe({
      next: t => { this.transactions.set(t); this.loading.set(false); },
      error: () => { this.error.set('Could not load transactions.'); this.loading.set(false); },
    });
  }

  raiseDispute(txnId: string) {
    this.router.navigate(['/disputes/new'], { queryParams: { transactionId: txnId } });
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
