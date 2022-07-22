using System.Collections.Generic;

namespace Warhammer.Application.Models.MatchViewModels
{
	public class AreaViewModel
	{
		public uint RecordsTotal { get; set; }
		public uint RecordsFiltered { get; set; }
		public int Draw { get; set; }
		public List<MatchViewModel> Data { get; set; }

		public AreaViewModel()
		{
		}

		public AreaViewModel(int draw, List<MatchViewModel> matches, uint totalItems)
		{
			this.Draw = draw;
			this.RecordsTotal = this.RecordsFiltered = totalItems;
			this.Data = matches;
		}
	}
}