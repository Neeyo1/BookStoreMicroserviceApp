import { useParamsStore } from "@/hooks/useParamsStore";
import { Button, ButtonGroup } from "flowbite-react";
import { AiOutlineArrowDown, AiOutlineArrowUp, AiOutlineSortAscending, AiOutlineSortDescending } from "react-icons/ai";

const pageSizeButtons = [5, 10, 15]
const orderButtons = [
    {
        label: "Name Ascending",
        icon: AiOutlineSortAscending,
        value: "name"
    },
    {
        label: "Name Descending",
        icon: AiOutlineSortDescending,
        value: "name-desc"
    },
    {
        label: "Count Ascending",
        icon: AiOutlineArrowUp,
        value: "count-desc"
    },
    {
        label: "Count Descending",
        icon: AiOutlineArrowDown,
        value: "count"
    }
]
const filterButtons = [
    {
        label: "All",
        value: "all"
    },
    {
        label: "Avaiable",
        value: "avaiable"
    },
    {
        label: "Non-avaiable",
        value: "non-avaiable"
    }
]

export default function Filters() {
  const pageSize = useParamsStore(state => state.pageSize);
  const setParams = useParamsStore(state => state.setParams);
  const orderBy = useParamsStore(state => state.orderBy);
  const filterBy = useParamsStore(state => state.filterBy);

  return (
    <div className="flex justify-between items-center mb-4">
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">
          Order by
        </span>
        <ButtonGroup>
          {orderButtons.map(({label, icon: Icon, value}) => (
            <Button
              key={value} 
              onClick={() => setParams({orderBy: value})}
              color={`${orderBy == value ? "red" : "gray"}`}
              className="focus:ring-0"
            >
              <Icon className="mr-3 h-4 w-4"/>
              {label}
            </Button>
          ))}
        </ButtonGroup>
      </div>
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">
          Filter by
        </span>
        <ButtonGroup>
          {filterButtons.map(({label, value}) => (
            <Button
              key={value} 
              onClick={() => setParams({filterBy: value})}
              color={`${filterBy == value ? "red" : "gray"}`}
              className="focus:ring-0"
            >
              {label}
            </Button>
          ))}
        </ButtonGroup>
      </div>
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">
          Page size
        </span>
        <ButtonGroup>
          {pageSizeButtons.map((value, index) => (
            <Button
              key={index} 
              onClick={() => setParams({pageSize: value})}
              color={`${pageSize == value ? "red" : "gray"}`}
              className="focus:ring-0"
            >
              {value}
            </Button>
          ))}
        </ButtonGroup>
      </div>
    </div>
  )
}
